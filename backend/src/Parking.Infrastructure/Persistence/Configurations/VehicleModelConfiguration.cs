namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class VehicleModelConfiguration : IEntityTypeConfiguration<VehicleModel>
{
    public void Configure(EntityTypeBuilder<VehicleModel> builder)
    {
        builder.ToTable("VehicleModel");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");
        builder.HasIndex(x => x.Name).IsUnique();

        // Seed com os modelos mais comuns no mercado brasileiro, para o catalogo ja nascer
        // util (o autocomplete do frontend so mostra sugestoes se houver o que sugerir).
        // Funcionarios continuam podendo cadastrar modelos novos que nao estejam aqui.
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var names = new[]
        {
            "VW Gol", "VW Polo", "VW Virtus", "VW T-Cross", "VW Nivus", "VW Saveiro",
            "Chevrolet Onix", "Chevrolet Onix Plus", "Chevrolet Tracker", "Chevrolet S10",
            "Fiat Argo", "Fiat Mobi", "Fiat Cronos", "Fiat Strada", "Fiat Toro", "Fiat Pulse",
            "Hyundai HB20", "Hyundai HB20S", "Hyundai Creta", "Hyundai Tucson",
            "Toyota Corolla", "Toyota Corolla Cross", "Toyota Yaris", "Toyota Hilux", "Toyota SW4",
            "Honda Civic", "Honda City", "Honda HR-V", "Honda Fit", "Honda CR-V",
            "Renault Kwid", "Renault Sandero", "Renault Logan", "Renault Duster",
            "Jeep Renegade", "Jeep Compass",
            "Nissan Kicks", "Nissan Versa", "Nissan Frontier",
            "Ford Ka", "Ford EcoSport", "Ford Ranger",
            "Citroen C3", "Peugeot 208",
            "Mitsubishi L200 Triton", "Volvo XC60", "BMW 320i", "Mercedes-Benz Classe A",
        };

        var seed = names
            .Select((name, index) => new
            {
                Id = (long)(index + 1),
                Name = name,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = (DateTime?)null,
            })
            .ToArray();

        builder.HasData(seed);
    }
}
