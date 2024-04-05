using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Commands;
using Dorbit.Framework.Commands.Abstractions;
using Dorbit.Identity.Contracts.Accesses;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;

namespace Dorbit.Identity.Commands
{
    [ServiceRegister]
    public class ImportAccessCommand : Command
    {
        private readonly AccessRepository _accessRepository;

        public override bool IsRoot { get; } = false;
        public override string Message => "Sync Policies";

        public ImportAccessCommand(AccessRepository accessRepository)
        {
            _accessRepository = accessRepository;
        }

        public override Task InvokeAsync(ICommandContext context)
        {
            var dbPolicies = _accessRepository.Set(false).ToList();

            async Task UpdatePolicies(IEnumerable<AccessImportRequest> items, Guid? parentId = null)
            {
                if (items is null) return;
                foreach (var item in items)
                {
                    var access = dbPolicies.FirstOrDefault(x => x.Name == item.Name);
                    if (access is null)
                    {
                        access = await _accessRepository.InsertAsync(new Access()
                        {
                            Name = item.Name,
                            ParentId = parentId
                        });
                    }
                    else
                    {
                        var adminPolicy = dbPolicies.FirstOrDefault(x => x.Name == "admin");
                        access.ParentId = parentId ?? adminPolicy?.Id;
                        access.IsDeleted = false;
                        access.DeletionTime = null;
                        if (access.Name == "admin") access.ParentId = null;
                        await _accessRepository.UpdateAsync(access);
                    }
                    
                    await UpdatePolicies(item.Children, access.Id);
                }
            }

            var content = File.ReadAllText(Path.Join(AppContext.BaseDirectory, "Assets/accesses.json"));
            var importPolicies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AccessImportRequest>>(content);
            return UpdatePolicies(importPolicies);
        }
    }
}