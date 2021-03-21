// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MSDF.DataChecker.Domain.Entities.Metadata
{
    public class ContainerEntityTypeConfiguration : IEntityTypeConfiguration<Container>
    {
        public void Configure(EntityTypeBuilder<Container> builder)
        {
            builder.HasOne(c => c.ContainerType)
                .WithMany()
                .HasForeignKey(c => c.ContainerTypeId);

            builder.HasOne(c => c.ParentContainer)
                .WithMany()
                .HasForeignKey(c => c.ParentContainerId);

            builder.HasMany(c => c.Containers)
                .WithOne(cc => cc.ParentContainer);
        }
    }
}
