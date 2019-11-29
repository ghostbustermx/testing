using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;

namespace Locus.Core.Services
{
    public enum Type
    {
        ACTIVES,
        INACTIVES
    };

    public interface IRunnersService
    {
        Runner Register(Runner runner);
        Runner GetRunner(Runner runner);

        List<RunnerDTO> GetInactives();
        List<RunnerDTO> GetActives();
        Runner Update(Runner runner);
        Runner Delete(Runner runner);
        Runner Disable(Runner runner);
        List<Runner> GetFullActives();
        Runner Get(int id);
    }

    public class RunnerService : IRunnersService
    {
        private readonly IRunnersRepository _runnerRepository;

        public RunnerService(IRunnersRepository runnerRepository)
        {
            _runnerRepository = runnerRepository;
        }

        public Runner Register(Runner runner)
        {
            runner.Creation_Date = DateTime.UtcNow;
            runner.Last_Connection_Date = DateTime.UtcNow;
            return _runnerRepository.Register(runner);
        }

        public Runner Update(Runner runner)
        {
            return _runnerRepository.Update(runner);
        }

        public Runner Delete(Runner runner)
        {
            return _runnerRepository.Delete(runner);
        }

        public Runner Disable(Runner runner)
        {
            return _runnerRepository.Disable(runner);
        }

        public List<RunnerDTO> GetInactives()
        {
            return this.GetRunners(Type.INACTIVES);
        }

        public List<RunnerDTO> GetActives()
        {
            return this.GetRunners(Type.ACTIVES);
        }


        private List<RunnerDTO> GetRunners(Type type)
        {
            List<Runner> runners = null;
            switch (type)
            {
                case Type.ACTIVES:
                    {
                        runners = _runnerRepository.GetActives();
                        break;
                    }
                case Type.INACTIVES:
                    {
                        runners = _runnerRepository.GetInactives();
                        break;
                    }
            }

            List<RunnerDTO> runnerList = new List<RunnerDTO>();
            foreach (var runner in runners)
            {
                TimeSpan t = DateTime.UtcNow.Subtract(runner.Last_Connection_Date);
                double time = t.TotalSeconds;
                RunnerDTO runnerDTO = new RunnerDTO()
                {
                    Identifier = runner.Identifier,
                    Id = runner.Id,
                    OS = runner.OS,
                    Tags = runner.Tags,
                    Creation_Date = runner.Creation_Date,
                    MAC = runner.MAC,
                    Description = runner.Description,
                    IPAddress = runner.IPAddress,
                    IsConnected = runner.IsConnected,
                    Status = runner.Status,
                    Last_Connection_Date = runner.Last_Connection_Date,
                    Time = time.ToString()
                };


                runnerDTO.Time = Math.Floor(time) + " Seconds";
                if (time >= 59.99)
                {
                    time = t.TotalMinutes;
                    string date = (time >= 2) ? " Minutes" : " Minute";
                    runnerDTO.Time = Math.Floor(time) + date;
                    if (time >= 59.99)
                    {
                        time = t.TotalHours;
                        date = (time >= 2) ? " Hours" : " Hour";
                        runnerDTO.Time = Math.Floor(time) + date;
                        if (time >= 23.99)
                        {
                            time = t.TotalDays;
                            date = (time >= 2) ? " Days" : " Day";
                            runnerDTO.Time = Math.Floor(time) + date;
                        }
                    }
                }
                runnerList.Add(runnerDTO);
            }
            return runnerList;
        }



        public Runner Get(int id)
        {
            return _runnerRepository.Get(id);
        }

        public Runner GetRunner(Runner runner)
        {
            return _runnerRepository.GetRunner(runner);
        }

        public List<Runner> GetFullActives()
        {
            return _runnerRepository.GetFullActives();
        }
    }
}
