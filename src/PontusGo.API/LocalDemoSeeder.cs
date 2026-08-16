using Microsoft.EntityFrameworkCore;
using PontusGo.Domain.Enums;
using PontusGo.Domain.Models;
using PontusGo.Infrastructure.Data;

namespace PontusGo.API;

public static class LocalDemoSeeder
{
    public static async Task SeedAsync(PontusGoDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        var admin = new User
        {
            Name = "Caio Martins",
            Email = "admin@pontusgo.demo",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.Admin
        };

        var student = new User
        {
            Name = "Marina Costa",
            Email = "aluno@pontusgo.demo",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Aluno@123"),
            Role = UserRole.Student
        };
        student.AddPoints(3_130);

        var cafeteria = CreateProduct(
            "Vale cafeteria",
            "Um lanche e uma bebida para recarregar as ideias.",
            650,
            12);
        var cinema = CreateProduct(
            "Ingresso de cinema",
            "Uma sessão para curtir qualquer lançamento.",
            1_200,
            16);
        var books = CreateProduct(
            "Kit de livros",
            "Três leituras para ampliar seu repertório.",
            1_800,
            8);
        var notebook = CreateProduct(
            "Caderno PontusGo",
            "Capa dura, pautado e pronto para novas ideias.",
            480,
            23);

        student.DeductPoints(cafeteria.PointsCost);
        cafeteria.DecreaseStock();

        var initialPoints = new PointTransaction(
            student.Id,
            3_130,
            "Saldo inicial do ambiente local")
        {
            PointsAwarded = 3_130,
            ActivityDescription = "Saldo inicial do ambiente local"
        };
        var redemption = new Redemption(
            student.Id,
            cafeteria.Id,
            cafeteria.PointsCost,
            "PG-8A2F-41C9")
        {
            PointsSpent = cafeteria.PointsCost
        };

        await context.Users.AddRangeAsync(admin, student);
        await context.Products.AddRangeAsync(cafeteria, cinema, books, notebook);
        await context.PointTransactions.AddAsync(initialPoints);
        await context.Redemptions.AddAsync(redemption);
        await context.SaveChangesAsync();
    }

    private static Product CreateProduct(
        string name,
        string description,
        int pointsCost,
        int stockQuantity)
    {
        var product = new Product
        {
            Name = name,
            Description = description,
            PointsCost = pointsCost
        };
        product.AddStock(stockQuantity);
        return product;
    }
}
