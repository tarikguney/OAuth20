using System;
using System.Collections.Generic;
using AuthorizationServer.Flows;
using AuthorizationServer.IdentityManagement;
using AuthorizationServer.TokenManagement;
using AuthorizationServer.UserManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorizationServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IClientManager, MockClientManager>();
            services.AddSingleton<IJwtGenerator, MockJwtGenerator>();
            services.AddSingleton<IAuthorizationCodeGenerator, MockAuthorizationCodeGenerator>();
            services.AddSingleton(AuthorizationFlowFactory);
            services.AddSingleton<IAuthorizationCodeValidator, AuthorizationCodeFlow>();
            services.AddSingleton<IUserCredentialValidator, MockUserCredentialValidator>();
        }

        private static IReadOnlyDictionary<AuthorizationFlowType, IGrantFlow> AuthorizationFlowFactory(IServiceProvider provider)
        {
            var jwtGenerator = provider.GetService<IJwtGenerator>();
            var authCodeGen = provider.GetService<IAuthorizationCodeGenerator>();
            return new Dictionary<AuthorizationFlowType, IGrantFlow>
            {
                {AuthorizationFlowType.Implicit, new ImplicitFlow(authCodeGen, jwtGenerator)},
                {AuthorizationFlowType.AuthorizationCode, new AuthorizationCodeFlow(authCodeGen)}
            };
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}