using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LazyWeChat;
using LazyWeChat.OfficialAccount;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LazyChatRoom
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
            services.AddControllersWithViews();
            services.AddLazyWeChat();
            services.AddSignalR();
            services.AddSingleton<IMessageStorage, MessageStorage>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseLazyWeChat(msg =>
            {
                if (msg.messageBody.Content.Contains("聊天") || msg.messageBody.Content.Contains("Chat"))
                {
                    var title = "LazyChatRoom";
                    var desc = "讨论微信、.Net Core、LazyWeChat等技术相关话题";
                    var picurl = "http://mmbiz.qpic.cn/mmbiz_jpg/xNJIgggnPOia9MXYaiaK0PhZvJ2uJ68Ftg6QGKtMZRPqbd8XLLukqSpOufoAb5ovgm5VzgqZib08DIP25mo1CA8xQ/0";
                    var url = "https://test.lazywechat.cn";
                    var list = new List<(string, string, string, string)>();
                    list.Add((title, desc, picurl, url));
                    msg.replyNewsMessage(list);
                }
                //msg.replyTextMessage("hello world");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=ChatRoom}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chathub");
            });
        }
    }
}
