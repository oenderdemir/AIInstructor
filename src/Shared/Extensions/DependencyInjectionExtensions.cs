using Couchbase.Extensions.DependencyInjection;
using Couchbase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Reflection;
using AIInstructor.src.Shared.Repository;
using AIInstructor.src.Shared.Service;

namespace AIInstructor.src.Shared.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddServicesAndRepositoriesFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes();

            var interfaces = types.Where(t => t.IsInterface);
            var classes = types.Where(t => t.IsClass && !t.IsAbstract);

            foreach (var @interface in interfaces)
            {
                var implementation = classes.FirstOrDefault(c =>
                    @interface.IsAssignableFrom(c)
                );

                if (implementation != null)
                {
                    services.AddScoped(@interface, implementation);
                }
            }
        }

        public static async Task<ICluster> ReplaceCouchbaseClusterAsync(this IServiceCollection services, string connectionString, string username, string password)
        {
            // ServiceProvider oluştur
            using var serviceProvider = services.BuildServiceProvider();

            // Eski cluster'ı dispose et
            var oldCluster = serviceProvider.GetService<ICluster>();
            if (oldCluster != null)
            {
                await oldCluster.DisposeAsync();
            }

            // Yeni cluster oluştur
            var newCluster = await Cluster.ConnectAsync(
                connectionString,
                new ClusterOptions
                {
                    UserName = username,
                    Password = password
                });

            // Eski kayıtları kaldır
            services.RemoveAll<ICluster>();
            services.RemoveAll<ICouchbaseLifetimeService>();

            // Yeni kayıtları ekle
            services.AddSingleton<ICluster>(newCluster);

            return newCluster;
        }


    }
}
