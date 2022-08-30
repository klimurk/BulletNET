using BulletNET.EntityFramework.Context.Configuration.Base;
using BulletNET.EntityFramework.Entities.Radar;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace BulletNET.EntityFramework.Context.Configuration;

//internal class RadarBoardConfiguration : DbEntityConfiguration<RadarBoard>
//{
//    public override void Configure(EntityTypeBuilder<RadarBoard> entity)
//    {
//        entity
//            .ToTable(typeof(RadarBoard).GetCustomAttribute(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute), false)?.ToString())
//            .HasKey(c => c.ID).HasMany(s=>s.TestGroups).WithOne().HasForeignKey()
//        // etc.
//    }
//}