

using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Common;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Model.Tasks;
using Slavestefan.Aphrodite.Web.Services;
using ApTask = Slavestefan.Aphrodite.Model.Tasks.Task;
using Task = System.Threading.Tasks.Task;

namespace Slavestefan.Aphrodite.Web.Modules
{
    [Group("Task")]
    public partial class TaskModule : AphroditeModuleBase
    {
        private readonly ILogger<TaskModule> _logger;
        private readonly TaskService _taskService;
        private readonly RelationshipService _relationshipService;

        public TaskModule(IServiceProvider services, ILogger<TaskModule> logger) : base(services)
        {
            _logger = logger;
            _relationshipService = new RelationshipService(TypedDbContext);
            _taskService = new TaskService(TypedDbContext);
        }

        [Command("Create")]
        public async Task CreateTaskSet(string name, bool doAllowMultiroll = true, bool doesMultirollRepeat = true)
        {
            try
            {
                if (TypedDbContext.TaskSets.Any(x => x.Name == name))
                {
                    await ReplySimpleEmbedAsync($"Name is already taken, please choose another.");
                    return;
                }

                var taskSet = new TaskSet()
                {
                    Name = name,
                    Owner = await TypedDbContext.GetOrCreateUserAsync(Context.Message.Author.Id, Context.Message.Author.Username),
                    DoAllowMultiroll = doAllowMultiroll,
                    DoesMultirollRepeat = doesMultirollRepeat
                };

                TypedDbContext.TaskSets.Add(taskSet);
                TypedDbContext.SaveChanges();
                await ReplySimpleEmbedAsync($"Taskset {name} has been created with Id: {taskSet.IdTaskSet}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not create Taskset {ex}");
            }
        }

        [Command("Add")]
        public async Task AddTask(string setNameOrId, [Remainder] string description)
        {
            try
            {
                var taskSet = _taskService.GetTaskSetFromNameOrId(setNameOrId);
                if (taskSet == null)
                {
                    await ReplySimpleEmbedAsync($"Taskset {setNameOrId} not found, task was not added.");
                    return;
                }

                if (taskSet.Owner.DiscordId != Context.User.Id && !await _relationshipService.ConfirmRelationship(Context.User.Id, taskSet.Owner.DiscordId, Model.Users.OwnerSlaveRelationshipTypes.Taskmaster))
                {
                    await ReplySimpleEmbedAsync($"You are not the owner of this taskset");
                }

                description = description.Trim('"');

                var task = new ApTask
                {
                    Description = description,
                };

                if (Context.Message.Attachments.Any())
                {
                    task.Image = new Uri(Context.Message.Attachments.First().Url);
                }

                taskSet.Tasks.Add(task);
                TypedDbContext.SaveChanges();
                await ReplySimpleEmbedAsync($"Task \"{description}\" was added to taskset {setNameOrId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not add task {ex}");
            }
        }

        [Command("Roll")]
        public async Task Roll(string setNameOrId, int amount = 1)
        {
            try
            {
                if (amount == 0)
                {
                    await ReplySimpleEmbedAsync("Haha, good one.");
                    return;
                }

                var multiSet = _taskService.GetMultiSetFromNameOrId(setNameOrId);
                var taskSet = _taskService.GetTaskSetFromNameOrId(setNameOrId);
                
                if (multiSet != null && taskSet != null)
                {
                    await ReplySimpleEmbedAsync($"Ambiguous name, please use one of the following Ids to roll: {taskSet.IdTaskSet} (single Task Roll) or {multiSet.IdMultiSet} (Multi Task Roll");
                    return;
                }

                if (multiSet != null)
                {
                    foreach (var taskRoll in multiSet.MultiSetTaskSets.Select(x => x.TaskSet))
                    {
                        await Roll(taskRoll.IdTaskSet.ToString());
                    }
                    return;
                }

                IList<ApTask> result;

                if (taskSet.DoesMultirollRepeat)
                {
                    result = new List<ApTask>();

                    for (int i = 0; i < amount; ++i)
                    {
                        result.Add(taskSet.Tasks.GetRandomNonRepeating(1).First());
                    }
                }
                else
                {
                    if (amount > taskSet.Tasks.Count)
                    {
                        await ReplySimpleEmbedAsync("Taskset does not have that many tasks");
                        return;
                    }

                    result = taskSet.Tasks.GetRandomNonRepeating(amount);
                }

                var user = await TypedDbContext.GetOrCreateUserAsync(Context.User.Id, Context.User.Username);
                foreach (var task in result)
                {
                    var taskHistory = new TaskHistory
                    {
                        Task = task,
                        Picker = user,
                        Time = DateTime.Now
                    };
                    TypedDbContext.TaskHistories.Add(taskHistory);
                    var embed = new EmbedBuilder
                    {
                        ImageUrl = task.Image?.ToString(),
                        Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "Roller",
                            Value = Context.User.Mention,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "Description",
                            Value = task.Description,
                            IsInline = true
                        },
                    }
                    };

                    TypedDbContext.SaveChanges();
                    await ReplySimpleEmbedAsync(embed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during task roll {setNameOrId}: {ex}");
            }
        }

        [Command("List")]
        public async Task List(string setNameOrId = null)
        {
            if (setNameOrId == null)
            {
                await ReplySimpleEmbedAsync(ListTaskSets());
                return;
            }

            var taskSet = _taskService.GetTaskSetFromNameOrId(setNameOrId);

            if (taskSet == null)
            {
                await ReplySimpleEmbedAsync($"Could not find set {setNameOrId}");
                return;
            }

            await ReplySimpleEmbedAsync(ListTasksFromSet(taskSet));
        }

        [Command("CreateMulti")]
        public async Task CreateMulti(string multiSetName)
        {
            try
            {
                if (TypedDbContext.MultiSet.Any(x => x.Name == multiSetName) || TypedDbContext.TaskSets.Any(x => x.Name == multiSetName))
                {
                    await ReplySimpleEmbedAsync($"Name is already taken, please choose another.");
                    return;
                }

                var multiSet = new MultiSet()
                {
                    Name = multiSetName,
                    Owner = await TypedDbContext.GetOrCreateUserAsync(Context.Message.Author.Id, Context.Message.Author.Username),
                };

                TypedDbContext.MultiSet.Add(multiSet);
                TypedDbContext.SaveChanges();
                await ReplySimpleEmbedAsync($"MultiSet {multiSetName} has been created with Id: {multiSet.IdMultiSet}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not create MultiSet {ex}");
            }
        }

        [Command("AddMulti")]
        public async Task AddToMultiTaskSet(string multiSetName, string taskSetName)
        {
            try
            {
                var multi = _taskService.GetMultiSetFromNameOrId(multiSetName);
                if (multi == null)
                {
                    await ReplySimpleEmbedAsync($"Could not find MultiSet with name {multiSetName}");
                    return;
                }

                var taskSet = _taskService.GetTaskSetFromNameOrId(taskSetName);
                if (taskSet == null)
                {
                    await ReplySimpleEmbedAsync($"Could not find TaskSet with name {taskSetName}");
                    return;
                }

                multi.MultiSetTaskSets.Add(new MultiSetTaskSet
                {
                    IdMultiSet = multi.IdMultiSet,
                    IdTaskSet = taskSet.IdTaskSet,
                    MultiSet = multi,
                    TaskSet = taskSet
                });

                TypedDbContext.SaveChanges();
                await ReplySimpleEmbedAsync($"Added Taskset with Id {taskSet.IdTaskSet} to MultiSet with Id {multi.IdMultiSet}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not add to Multiset {ex}");
                await ReplySimpleEmbedAsync("An error has occured");
            }
        }

        private EmbedBuilder ListTaskSets()
        {
            var sets = TypedDbContext.TaskSets.AsQueryable().Where(x => x.Owner.DiscordId == Context.User.Id);

            var embed = new EmbedBuilder
            {
                Description = "Your task sets " + Context.User.Mention,
                Fields = new List<EmbedFieldBuilder>()
            };

            foreach (var item in sets)
            {
                var embedField = new EmbedFieldBuilder
                {
                    Name = item.IdTaskSet.ToString(),
                    Value = item.Name
                };
                embed.Fields.Add(embedField);
            }

            return embed;
        }

        private EmbedBuilder ListTasksFromSet(TaskSet taskSet)
        {
            var embed = new EmbedBuilder
            {
                Description = $"Tasks in set {taskSet.Name}",
                Footer = new EmbedFooterBuilder
                {
                    Text = $"Request by {Context.User.Username}#{Context.User.Discriminator}"
                },
                Fields = new List<EmbedFieldBuilder>()
            };

            foreach (var item in taskSet.Tasks)
            {
                var embedField = new EmbedFieldBuilder
                {
                    IsInline = false,
                    Name = item.IdTask.ToString(),
                    Value = item.Description
                };
                embed.Fields.Add(embedField);
            }

            return embed;
        }


    }
}