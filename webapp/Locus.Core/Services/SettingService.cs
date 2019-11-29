using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;


namespace Locus.Core.Services
{

    public interface ISettingsService
    {
        Setting Save(Setting settings);
        Setting GetByName(Setting settings);
        Setting Update(Setting settings);
    }

    public class SettingService : ISettingsService
    {
        private readonly ISettingsRepository _settingsRepository;

        public SettingService(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public Setting GetByName(Setting setting)
        {
            return _settingsRepository.GetByName(setting);
        }

        public Setting Save(Setting settings)
        {
            return _settingsRepository.Save(settings);
        }

        public Setting Update(Setting settings)
        {
            return _settingsRepository.Update(settings);
        }
    }
}
