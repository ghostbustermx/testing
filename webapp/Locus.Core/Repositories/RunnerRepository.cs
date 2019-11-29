using Locus.Core.Context;
using Locus.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Locus.Core.Repositories
{

    public interface IRunnersRepository
    {
        Runner Register(Runner runner);
        Runner GetRunner(Runner runner);
        List<Runner> GetActives();
        List<Runner> GetFullActives();
        List<Runner> GetInactives();
        Runner Update(Runner runner);
        Runner Delete(Runner runner);
        Runner Disable(Runner runner);
        Runner Get(int id);
    }

    public class RunnerRepository : IRunnersRepository
    {

        //Instance of Database Context
        private LocustDBContext context = new LocustDBContext();

        public Runner Delete(Runner runner)
        {
            try
            {
                context.Runners.Remove(runner);
                context.SaveChanges();
                return runner;
            }
            catch
            {
                return null;
            }
        }

        public Runner Disable(Runner runner)
        {
            runner.Status = false;
            try
            {
                context.Entry(runner).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return runner;
            }
            catch
            {
                return null;
            }

        }

        public Runner Get(int id)
        {
            try
            {
                return context.Runners.Find(id);
            }
            catch
            {
                return null;
            }
        }

        public List<Runner> GetActives()
        {
            try
            {
                // return context.Runners.Where(runner => runner.IsConnected == true && runner.Status == true).ToList();

                return context.Runners.Where(runner => runner.Status == true).ToList();
            }
            catch
            {

                return null;
            }
        }

        public List<Runner> GetAll()
        {
            try
            {
                return context.Runners.Where(r => r.Status == true).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<Runner> GetFullActives()
        {
            return context.Runners.Where(runner => runner.IsConnected == true && runner.Status == true).ToList();
        }

        public List<Runner> GetInactives()
        {
            try
            {
                return context.Runners.Where(r => r.Status == false).ToList();
            }
            catch
            {
                return null;
            }
        }

        public Runner GetRunner(Runner runner)
        {
            try
            {
                return context.Runners.Where(r => r.Identifier == runner.Identifier).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public Runner Register(Runner runner)
        {
            try
            {
                context.Runners.Add(runner);
                context.SaveChanges();
                return runner;
            }
            catch
            {
                return null;
            }

        }

        public Runner Update(Runner runner)
        {
            try
            {
                context.Entry(runner).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return runner;
            }
            catch
            {
                return null;
            }
        }
    }
}
