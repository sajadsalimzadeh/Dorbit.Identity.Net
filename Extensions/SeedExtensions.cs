using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Extensions;

public static class SeedExtensions
{
    private static object _seedLock = new { };

    public static Task SeedAccessAsync(this AccessRepository accessRepository, string filename)
    {
        lock (_seedLock)
        {
            var allAccesses = accessRepository.Set().Include(x => x.Parent).ToList();

            async Task ImportAsync(ICollection<Access> accesses, Access parent)
            {
                if (accesses is null) return;
                foreach (var access in accesses)
                {
                    var children = access.Children;
                    access.Children = null;
                    var existsParent = allAccesses.FirstOrDefault(x => x.Name == access.Name && x.Parent?.Name == parent?.Name);
                    if (existsParent is not null)
                    {
                        await ImportAsync(children, existsParent);
                    }
                    else
                    {
                        access.ParentId = parent?.Id;
                        var entity = await accessRepository.InsertAsync(access);
                        await ImportAsync(children, entity);
                    }
                }
            }

            var accessFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Assets/{filename}.json");
            if (File.Exists(accessFilename))
            {
                var json = File.ReadAllTextAsync(accessFilename).Result;
                var accesses = JsonSerializer.Deserialize<List<Access>>(json);
                ImportAsync(accesses, null).Wait();
            }
        }
        return Task.CompletedTask;
    }
}