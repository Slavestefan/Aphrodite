using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Model.Tasks;
using ApTask = Slavestefan.Aphrodite.Model.Tasks.Task;

namespace Slavestefan.Aphrodite.Web.Services
{
    public class TaskService
    {
        private readonly PicAutoPostContext _context;

        public TaskService(PicAutoPostContext context)
        {
            _context = context;
        }

        public ApTask Roll(TaskSet taskset)
        {
            throw new NotImplementedException();
        }

        public MultiSet GetMultiSetFromNameOrId(string nameOrId)
        {
            if (Guid.TryParse(nameOrId, out var guid))
            {
                return _context.MultiSet.Include(x => x.MultiSetTaskSets).ThenInclude(x => x.TaskSet).Include(x => x.Owner).FirstOrDefault(x => x.IdMultiSet == guid);
            }
            else
            {
                return _context.MultiSet.Include(x => x.MultiSetTaskSets).ThenInclude(x => x.TaskSet).Include(x => x.Owner).FirstOrDefault(x => x.Name == nameOrId);
            }
        }

        public TaskSet GetTaskSetFromNameOrId(string nameOrId)
        {
            if (Guid.TryParse(nameOrId, out var guid))
            {
                return _context.TaskSets.Include(x => x.Tasks).Include(x => x.Owner).FirstOrDefault(x => x.IdTaskSet == guid);
            }
            else
            {
                return _context.TaskSets.Include(x => x.Tasks).Include(x => x.Owner).FirstOrDefault(x => x.Name == nameOrId);
            }
        }
    }
}