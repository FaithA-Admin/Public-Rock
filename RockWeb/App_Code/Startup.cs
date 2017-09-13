﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using Microsoft.Owin;
using Owin;
using System.Web;
using React;

[assembly: OwinStartup(typeof(RockWeb.Startup))]
namespace RockWeb
{
    public class Startup
    {
        public void Configuration( IAppBuilder app )
        {
            app.MapSignalR();
            ReactSiteConfiguration.Configuration = new ReactSiteConfiguration()
                .AddScript("~/Scripts/React/dist/server.blocks.bundle.js");
        }
    }
}
