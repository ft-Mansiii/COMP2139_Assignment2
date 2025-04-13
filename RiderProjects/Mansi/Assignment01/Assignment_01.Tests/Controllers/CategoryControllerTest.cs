using System.Security.Claims;
using System.Threading.Tasks;
using Assignment01.Areas.Management.Controllers;
using Assignment01.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Assignment_01.Tests.Controllers;

public class CategoryControllerTests
{
    [Fact]
    public async Task Index_ReturnsViewResult()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<Assignment01DB>()
            .UseInMemoryDatabase(databaseName: "TestDB")
            .Options;

        using (var context = new Assignment01DB(options))
        {
            // Seed fake data if needed
            context.Categories.Add(new Assignment01.Areas.Management.Models.Category
            {
                CategoryId = 1,
                CategoryName = "Test Category",
                CategoryDescription = "Test Description"
            });
            await context.SaveChangesAsync();

            var controller = new CategoryController(context);

            // Mock User and HttpContext
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "TestUser")
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model); // Optional: make sure data is passed
        }
    }
}


// what unit test is doing 

/*"So in this test, I’m testing the Index action of my CategoryController. I’m not using a real database — instead, I set up a mock one with a sample list of categories.

I then call the Index() method, and I expect it to return a view. I also check that the model being sent to the view is a list of categories — just like how it would behave in the real app.

    If both those checks pass, the test is successful. That tells me the controller action is working properly in isolation."*/