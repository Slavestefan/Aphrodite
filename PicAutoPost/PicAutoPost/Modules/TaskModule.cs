

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
using ApTask = Slavestefan.Aphrodite.Model.Tasks.Task;
using Task = System.Threading.Tasks.Task;

namespace Slavestefan.Aphrodite.Web.Modules
{
    [Group("Task")]
    public class TaskModule : AphroditeModuleBase
    {
        private readonly ILogger<TaskModule> _logger;

        public TaskModule(IServiceProvider services, ILogger<TaskModule> logger) : base(services)
        {
            _logger = logger;
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
        public async Task AddTask(string setNameOrId, string description)
        {
            try
            {
                var taskSet = GetTaskSetFromNameOrId(setNameOrId);
                if (taskSet == null)
                {
                    await ReplySimpleEmbedAsync($"Taskset {setNameOrId} not found, task was not added.");
                    return;
                }

                if (taskSet.Owner.DiscordId != Context.User.Id)
                {
                    await ReplySimpleEmbedAsync($"You are not the owner of this taskset");
                }

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
                await ReplySimpleEmbedAsync($"Task {description} was added to taskset {setNameOrId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not add task {ex}");
            }
        }

        [Command("Roll")]
        public async Task Roll(string setNameOrId, int amount = 1)
        {
            if (amount == 0)
            {
                await ReplySimpleEmbedAsync("Haha, good one.");
                return;
            }

            var taskSet = GetTaskSetFromNameOrId(setNameOrId);
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
            

            foreach (var task in result)
            {
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

                await ReplyAsync(embed: embed.Build());
            }
        }

        [Command("List")]
        public async Task List(string setNameOrId = null)
        {
            if (setNameOrId == null)
            {
                await ReplyAsync(embed: ListTaskSets());
            }

            var taskSet = GetTaskSetFromNameOrId(setNameOrId);

            if (taskSet == null)
            {
                await ReplySimpleEmbedAsync($"Could not find set {setNameOrId}");
                return;
            }

            await ReplyAsync(embed: ListTasksFromSet(taskSet));
        }

        private Embed ListTaskSets()
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

            return embed.Build();
        }

        private Embed ListTasksFromSet(TaskSet taskSet)
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

            return embed.Build();
        }

        private TaskSet GetTaskSetFromNameOrId(string nameOrId)
        {
            if(Guid.TryParse(nameOrId, out var guid))
            {
                return TypedDbContext.TaskSets.Include(x => x.Tasks).Include(x => x.Owner).FirstOrDefault(x => x.IdTaskSet == guid);
            }
            else
            {
                return TypedDbContext.TaskSets.Include(x => x.Tasks).Include(x => x.Owner).FirstOrDefault(x => x.Name == nameOrId);
            }
        }
    }
}