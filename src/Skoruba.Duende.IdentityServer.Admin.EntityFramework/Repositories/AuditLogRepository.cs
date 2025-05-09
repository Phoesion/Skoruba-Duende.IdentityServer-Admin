﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Skoruba.AuditLogging.EntityFramework.DbContexts;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Entities;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Extensions.Common;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Extensions.Enums;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Extensions.Extensions;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Repositories.Interfaces;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Repositories
{
    public class AuditLogRepository<TDbContext, TAuditLog> : IAuditLogRepository<TAuditLog>
        where TDbContext : IAuditLoggingDbContext<TAuditLog>
        where TAuditLog : AuditLog
    {
        protected readonly TDbContext DbContext;

        public AuditLogRepository(TDbContext dbContext)
        {
            DbContext = dbContext;
        }
        
        public async Task<List<DashboardAuditLogDataView>> GetDashboardAuditLogsAsync(int lastNumberOfDays, CancellationToken cancellationToken = default)
        {
            var logs = await DbContext.AuditLog
                .Where(x => x.Created > DateTime.Now.AddDays(-lastNumberOfDays))
                .GroupBy(x => x.Created.Date)
                .OrderBy(x => x.Key)
                .Select(x => new DashboardAuditLogDataView
                {
                    Created = x.Key,
                    Total = x.Count()
                })
                .ToListAsync(cancellationToken);

            return logs;
        }
    
        public async Task<int> GetDashboardAuditLogsAverageAsync(int lastNumberOfDays, CancellationToken cancellationToken = default)
        {
            var dailyCounts = await DbContext.AuditLog
                .Where(a => a.Created >= DateTime.Now.AddDays(-lastNumberOfDays))
                .GroupBy(a => a.Created.Date)
                .Select(g => g.Count())
                .ToListAsync(cancellationToken: cancellationToken);

            if (dailyCounts.Count > 0)
            {
                return (int)dailyCounts.Average();
            }

            return 0;
        }

        public async Task<PagedList<TAuditLog>> GetAsync(
            string @event,
            string source,
            string category,
            DateOnly? created,
            string subjectIdentifier,
            string subjectName,
            int page = 1,
            int pageSize = 10)
        {
            var pagedList = new PagedList<TAuditLog>();

            var query = DbContext.AuditLog
                .WhereIf(!string.IsNullOrEmpty(subjectIdentifier), log => log.SubjectIdentifier.Contains(subjectIdentifier))
                .WhereIf(!string.IsNullOrEmpty(subjectName), log => log.SubjectName.Contains(subjectName))
                .WhereIf(!string.IsNullOrEmpty(@event), log => log.Event.Contains(@event))
                .WhereIf(!string.IsNullOrEmpty(source), log => log.Source.Contains(source))
                .WhereIf(!string.IsNullOrEmpty(category), log => log.Category.Contains(category));

            if (created.HasValue)
            {
                var from = created.Value.ToDateTime(TimeOnly.MinValue);
                var to = created.Value.ToDateTime(TimeOnly.MaxValue);

                query = query.Where(log => log.Created >= from && log.Created <= to);
            }

            pagedList.TotalCount = await query.CountAsync();

            var auditLogs = await query
                .PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(auditLogs);
            pagedList.PageSize = pageSize;

            return pagedList;
        }


        public virtual async Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan)
        {
            var logsToDelete = await DbContext.AuditLog.Where(x => x.Created.Date < deleteOlderThan.Date).ToListAsync();

            if (logsToDelete.Count == 0) return;

            DbContext.AuditLog.RemoveRange(logsToDelete);

            await AutoSaveChangesAsync();
        }

        protected virtual async Task<int> AutoSaveChangesAsync()
        {
            return AutoSaveChanges ? await DbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
        }

        public bool AutoSaveChanges { get; set; } = true;

        public virtual async Task<int> SaveAllChangesAsync()
        {
            return await DbContext.SaveChangesAsync();
        }
    }
}
