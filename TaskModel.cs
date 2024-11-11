
using SQLite;
using System.Threading.Tasks;

namespace TaskTracker
{
    public class TaskModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Difficulty { get; set; }
        public string Term { get; set; }

        public override string ToString()
        {
            return $"{Name}";

        }        
        public string[] ret()
        {
            string[] arr = new string[4] { Name, Description, Difficulty, Term } ;
            return arr;

        }
    }

    public class TaskDataBase
    {
        private readonly SQLiteAsyncConnection _db;

        public TaskDataBase(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<TaskModel>().Wait();
        }

        public Task<List<TaskModel>> GetTasksAsync() 
        { 
            return _db.Table<TaskModel>().ToListAsync();
        }

        public Task<int> SaveTaskAsync(TaskModel task)
        {
            if (task.Id != 0)
                return _db.UpdateAsync(task);
            else
                return _db.InsertAsync(task);
        }

        public Task<int> DeleteTaskAsync(TaskModel task)
        {
            return _db.DeleteAsync(task);
        }

    }

}
