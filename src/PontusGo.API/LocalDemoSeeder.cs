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
            Role = UserRole.Admin,
            TuitionStatus = TuitionStatus.UpToDate
        };

        var student1 = new User
        {
            Name = "Marina Costa",
            Email = "aluno@pontusgo.demo",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Aluno@123"),
            Role = UserRole.Student,
            TuitionStatus = TuitionStatus.UpToDate
        };
        student1.AddPoints(3_130);

        var student2 = new User
        {
            Name = "Lucas Silva",
            Email = "lucas@pontusgo.demo",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Aluno@123"),
            Role = UserRole.Student,
            TuitionStatus = TuitionStatus.Pending
        };
        student2.AddPoints(180);

        var student3 = new User
        {
            Name = "Beatriz Santos",
            Email = "beatriz@pontusgo.demo",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Aluno@123"),
            Role = UserRole.Student,
            TuitionStatus = TuitionStatus.Overdue
        };
        student3.AddPoints(90);

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

        student1.DeductPoints(cafeteria.PointsCost);
        cafeteria.DecreaseStock();

        var initialPoints1 = new PointTransaction(
            student1.Id,
            3_130,
            "Saldo inicial do ambiente local")
        {
            PointsAwarded = 3_130,
            ActivityDescription = "Saldo inicial do ambiente local"
        };

        var initialPoints2 = new PointTransaction(
            student2.Id,
            180,
            "Assiduidade (+10 pts), Participação (+10 pts), Fazer Tarefa (+10 pts)")
        {
            PointsAwarded = 180,
            ActivityDescription = "Assiduidade (+10 pts), Participação (+10 pts), Fazer Tarefa (+10 pts)"
        };

        var initialPoints3 = new PointTransaction(
            student3.Id,
            90,
            "Assiduidade (+10 pts), Participação (+10 pts)")
        {
            PointsAwarded = 90,
            ActivityDescription = "Assiduidade (+10 pts), Participação (+10 pts)"
        };

        var redemption = new Redemption(
            student1.Id,
            cafeteria.Id,
            cafeteria.PointsCost,
            "PG-8A2F-41C9")
        {
            PointsSpent = cafeteria.PointsCost
        };

        await context.Users.AddRangeAsync(admin, student1, student2, student3);
        await context.Products.AddRangeAsync(cafeteria, cinema, books, notebook);
        await context.PointTransactions.AddRangeAsync(initialPoints1, initialPoints2, initialPoints3);
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
