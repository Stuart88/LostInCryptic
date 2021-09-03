using LostInCryptic.Server.DataContext;
using LostInCryptic.Server.Extensions;
using LostInCryptic.Shared.DbModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LostInCryptic.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();
            services.AddRazorPages();
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            PrepareDb();

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                //endpoints.MapFallbackToFile("index.html");
                endpoints.MapFallbackToFile("{**slug}", "index.html");
            });
        }

        private void PrepareDb()
        {
            using (WebsiteDbContext context = new WebsiteDbContext())
            {
                context.Database.EnsureCreated();

                // Add words if no data. These words are used for creating random urlCode for each question.
                if (!context.UrlCodes.Any())
                {
                    var words = File.ReadAllLines("wwwroot/Assets/englishDictionary.csv")
                        .Where(w => w.Length == 10 && w.All(c => char.IsLetter(c)))
                        .Distinct()
                        .Select(s => new Word() { Value = s});
                    
                    context.UrlCodes.AddRange(words);
                    context.SaveChanges();
                }


                // Add test questions if no data

                if (!context.Questions.Any())
                {
                    var q1 = new Question
                    {
                        QuestionText = "What is three plus three?",
                        QuestionAnswer = "Three"
                    };
                    q1.AssignUrlCode(context);
                    context.Add(q1);
                    context.SaveChanges();

                    var q2 = new Question
                    {
                        QuestionText = "Who are you?",
                        QuestionAnswer = "Me"
                    };
                    q2.AssignUrlCode(context);
                    context.Add(q2);
                    context.SaveChanges();
                }
            }
        }
    }
}
