using Microsoft.EntityFrameworkCore;
using PontusGo.Domain.Enums;
using PontusGo.Domain.Models;
using PontusGo.Infrastructure.Data;

namespace PontusGo.API;

public static class LocalDemoSeeder
{
    public static async Task SeedAsync(PontusGoDbContext context)
    {
        var hasChanges = false;

        // 1. Garantir que o Administrador exista
        if (!await context.Users.AnyAsync(u => u.Email == "admin@pontusgo.demo"))
        {
            var admin = new User
            {
                Name = "Caio Martins",
                Email = "admin@pontusgo.demo",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
                TuitionStatus = TuitionStatus.UpToDate
            };
            await context.Users.AddAsync(admin);
            hasChanges = true;
        }

        // 2. Garantir que o Professor exista
        if (!await context.Users.AnyAsync(u => u.Email == "professor@pontusgo.demo"))
        {
            var teacher = new User
            {
                Name = "Helena Fernandes (Professora)",
                Email = "professor@pontusgo.demo",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Prof@123"),
                Role = UserRole.Teacher,
                TuitionStatus = TuitionStatus.UpToDate
            };
            await context.Users.AddAsync(teacher);
            hasChanges = true;
        }

        // 3. Garantir os Estudantes Demo
        if (!await context.Users.AnyAsync(u => u.Email == "aluno@pontusgo.demo"))
        {
            var student1 = new User
            {
                Name = "Marina Costa",
                Email = "aluno@pontusgo.demo",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Aluno@123"),
                Role = UserRole.Student,
                TuitionStatus = TuitionStatus.UpToDate
            };
            student1.AddPoints(3_130);
            await context.Users.AddAsync(student1);

            var initialPoints1 = new PointTransaction(
                student1.Id,
                3_130,
                "Saldo inicial do ambiente local")
            {
                PointsAwarded = 3_130,
                ActivityDescription = "Saldo inicial do ambiente local"
            };
            await context.PointTransactions.AddAsync(initialPoints1);
            hasChanges = true;
        }

        if (!await context.Users.AnyAsync(u => u.Email == "lucas@pontusgo.demo"))
        {
            var student2 = new User
            {
                Name = "Lucas Silva",
                Email = "lucas@pontusgo.demo",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Aluno@123"),
                Role = UserRole.Student,
                TuitionStatus = TuitionStatus.Pending
            };
            student2.AddPoints(180);
            await context.Users.AddAsync(student2);

            var initialPoints2 = new PointTransaction(
                student2.Id,
                180,
                "Assiduidade (+10 pts), Participação (+10 pts), Fazer Tarefa (+10 pts)")
            {
                PointsAwarded = 180,
                ActivityDescription = "Assiduidade (+10 pts), Participação (+10 pts), Fazer Tarefa (+10 pts)"
            };
            await context.PointTransactions.AddAsync(initialPoints2);
            hasChanges = true;
        }

        if (!await context.Users.AnyAsync(u => u.Email == "beatriz@pontusgo.demo"))
        {
            var student3 = new User
            {
                Name = "Beatriz Santos",
                Email = "beatriz@pontusgo.demo",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Aluno@123"),
                Role = UserRole.Student,
                TuitionStatus = TuitionStatus.Overdue
            };
            student3.AddPoints(90);
            await context.Users.AddAsync(student3);

            var initialPoints3 = new PointTransaction(
                student3.Id,
                90,
                "Assiduidade (+10 pts), Participação (+10 pts)")
            {
                PointsAwarded = 90,
                ActivityDescription = "Assiduidade (+10 pts), Participação (+10 pts)"
            };
            await context.PointTransactions.AddAsync(initialPoints3);
            hasChanges = true;
        }

        // 4. Garantir Produtos do Catálogo
        if (!await context.Products.AnyAsync())
        {
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

            await context.Products.AddRangeAsync(cafeteria, cinema, books, notebook);
            hasChanges = true;
        }

        if (hasChanges)
        {
            await context.SaveChangesAsync();
        }
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
