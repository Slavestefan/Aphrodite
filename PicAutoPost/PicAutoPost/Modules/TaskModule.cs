

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Slavestefan.Aphrodite.Model;
using ApTask = Slavestefan.Aphrodite.Model.Tasks.Task;

namespace Slavestefan.Aphrodite.Web.Modules
{
    [Group("Task")]
    public class TaskModule : AphroditeModuleBase
    {
        public TaskModule(IServiceProvider services) : base(services)
        {
        }

        [Command("Add")]
        public async Task AddTask(string name, string description, string hiddenDescription = null)
        {
            var existingTask = TypedDbContext.Tasks.FirstOrDefault(x => x.Name == name && x.Owner.DiscordId == Context.User.Id);
            if (existingTask != null)
            {
                await ReplySimpleEmbedAsync($"Task with that name already exists: {existingTask.Description}");
                return;
            }

            var task = new ApTask()
            {
                Description = description,
                HiddenDescription = hiddenDescription,
                IdTask = Guid.NewGuid(),
                Name = name,
                Owner = await TypedDbContext.GetOrCreateUserAsync(Context.Message.Author.Id, Context.Message.Author.Username)
            };

            TypedDbContext.Tasks.Add(task);
            TypedDbContext.SaveChanges();
            await ReplySimpleEmbedAsync($"Task has been created with Id: {task.IdTask}");
        }

        [Command("List")]
        public async Task ListTasks(string configName = null)
        {
            var tasks = configName == null
                ? TypedDbContext.Tasks.Where(x => x.Owner.DiscordId == Context.User.Id)
                : (await TypedDbContext.TaskConfigurations.FirstOrDefaultAsync(x => x.Name == configName))?.Tasks.Where(x => true);

            if (tasks == null || !tasks.Any())
            {
                await ReplySimpleEmbedAsync("No tasks found");
            }

        }
    }
}