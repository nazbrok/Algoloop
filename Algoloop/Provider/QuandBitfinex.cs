﻿/*
 * Copyright 2019 Capnode AB
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Algoloop.Model;
using QuantConnect.Configuration;
using QuantConnect.ToolBox.QuandlBitfinexDownloader;
using System;

namespace Algoloop.Provider
{
    public class QuandBitfinex : IProvider
    {
        public void Download(MarketModel model, SettingModel settings)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            Config.Set("log-handler", "QuantConnect.Logging.CompositeLogHandler");
            Config.Set("data-directory", settings.DataFolder);

            string apiKey = ""; // TODO:
            QuandlBitfinexDownloaderProgram.QuandlBitfinexDownloader(model.LastDate, apiKey);
        }
    }
}
