using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerRest.Controllers;
using ServerRest.DataBase;

namespace ServerRest.Test;

[TestClass]
public class UserControllerTests
{
    private DbContextOptions<AppDbContext> _options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;

    private AppDbContext _dbContext;

    [TestInitialize]
    public void InitializeTest()
    {
        _dbContext = new AppDbContext(_options);
    }

    [TestCleanup]
    public void CleanupTest()
    {
        _dbContext.Database.EnsureDeleted();
    }

    [TestMethod]
    public async Task Get_ShouldReturnUsersList_WhenSuccessfullyInvoked()
    {
        // Arrange
        var user1 = new UserTable {Id = 1, Name = "user1"};
        var user2 = new UserTable {Id = 2, Name = "user2"};
        var user3 = new UserTable {Id = 3, Name = "user3"};
        _dbContext.UserTable.AddRange(user1, user2, user3);
        await _dbContext.SaveChangesAsync();

        var userController = new UserController(_dbContext);

        // Act
        var actionResult = await userController.Get();
        var result = actionResult as OkObjectResult;
        var usersList = ((IEnumerable<UserTable>) result.Value).ToList();

        // Assert
        Assert.AreEqual(usersList.Count, 3);
        Assert.AreEqual(usersList[0].Id, 1);
        Assert.AreEqual(usersList[0].Name, "user1");
        Assert.AreEqual(usersList[1].Id, 2);
        Assert.AreEqual(usersList[1].Name, "user2");
        Assert.AreEqual(usersList[2].Id, 3);
        Assert.AreEqual(usersList[2].Name, "user3");
    }

    [TestMethod]
    public async Task Get_ShouldReturnUser_WhenSuccessfullyInvoked()
    {
        // Arrange
        var user1 = new UserTable {Id = 1, Name = "user1"};
        _dbContext.UserTable.Add(user1);
        await _dbContext.SaveChangesAsync();

        var userController = new UserController(_dbContext);

        // Act
        var actionResult = await userController.Get(1);
        var result = actionResult as OkObjectResult;
        var user = ((UserTable) result.Value);

        // Assert
        Assert.AreEqual(user.Id, 1);
        Assert.AreEqual(user.Name, "user1");
    }

    [TestMethod]
    public async Task Get_ShouldReturnNotFound_WhenUserNotFound()
    {
        // Arrange
        var user1 = new UserTable { Id = 1, Name = "user1" };
        _dbContext.UserTable.Add(user1);
        await _dbContext.SaveChangesAsync();

        var userController = new UserController(_dbContext);

        // Act
        var actionResult = await userController.Get(2);
        var result = actionResult as NotFoundResult;

        // Assert
        Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
    }

    [TestMethod]
    public async Task Post_ShouldCreateUser_WhenSuccessfullyInvoked()
    {
        // Arrange
        var newUser = new UserTable { Name = "user1" };

        var userController = new UserController(_dbContext);

        // Act
        var actionResult = await userController.Post(newUser);
        var result = actionResult as CreatedAtActionResult;
        var createdUser = ((UserTable)result.Value);

        // Assert
        Assert.AreEqual(createdUser.Id, 1);
        Assert.AreEqual(createdUser.Name, newUser.Name);
    }

    [TestMethod]
    public async Task Post_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var newUser = new UserTable { Name = "" };

        var userController = new UserController(_dbContext);
        userController.ModelState.AddModelError("Name", "Name is required");

        // Act
        var actionResult = await userController.Post(newUser);
        var result = actionResult as BadRequestObjectResult;

        // Assert
        Assert.AreEqual(result.StatusCode, StatusCodes.Status400BadRequest);
    }

    [TestMethod]
    public async Task Put_ShouldUpdateUser_WhenSuccessfullyInvoked()
    {
        // Arrange
        var user1 = new UserTable { Id = 1, Name = "user1" };
        _dbContext.UserTable.Add(user1);
        await _dbContext.SaveChangesAsync();


        user1.Name = "user1_modified";

        var userController = new UserController(_dbContext);

        // Act
        var actionResult = await userController.Put(1, user1);
        var result = actionResult as OkObjectResult;
        var updatedUser = ((UserTable)result.Value);

        // Assert
        Assert.AreEqual(updatedUser.Id, user1.Id);
        Assert.AreEqual(updatedUser.Name, user1.Name);
    }

    [TestMethod]
    public async Task Put_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var user1 = new UserTable { Id = 1, Name = "user1" };
        _dbContext.UserTable.Add(user1);
        await _dbContext.SaveChangesAsync();

        var modifiedUser1 = new UserTable { Id = 1, Name = "" };

        var userController = new UserController(_dbContext);
        userController.ModelState.AddModelError("Name", "Name is required");

        // Act
        var actionResult = await userController.Put(1, modifiedUser1);
        var result = actionResult as BadRequestObjectResult;

        // Assert
        Assert.AreEqual(result.StatusCode,StatusCodes.Status400BadRequest);
    }

    [TestMethod]
    public async Task Put_ShouldReturnBadRequest_WhenIdsDontMatch()
    {
        // Arrange
        var user1 = new UserTable { Id = 1, Name = "user1" };
        _dbContext.UserTable.Add(user1);
        await _dbContext.SaveChangesAsync();

        var modifiedUser1 = new UserTable { Id = 2, Name = "user1_modified" };

        var userController = new UserController(_dbContext);

        // Act
        var actionResult = await userController.Put(1, modifiedUser1);
        var result = actionResult as BadRequestResult;

        // Assert
        Assert.AreEqual(result.StatusCode, StatusCodes.Status400BadRequest);
    }

    [TestMethod]
    public async Task Delete_ShouldDeleteUser_WhenSuccessfullyInvoked()
    {
        // Arrange
        var user1 = new UserTable { Id = 1, Name = "user1" };
        _dbContext.UserTable.Add(user1);
        await _dbContext.SaveChangesAsync();

        var userController = new UserController(_dbContext);

        // Act
        var actionResult = await userController.Delete(1);
        var result = actionResult as OkObjectResult;
        var deletedUser = ((UserTable)result.Value);

        // Assert
        Assert.AreEqual(deletedUser.Id, user1.Id);
        Assert.AreEqual(deletedUser.Name, user1.Name);
        Assert.AreEqual(_dbContext.UserTable.Count(), 0);
    }

    [TestMethod]
    public async Task Delete_ShouldReturnNotFound_WhenUserDoesntExists()
    {
        // Arrange
        var user1 = new UserTable { Id = 1, Name = "user1" };
        _dbContext.UserTable.Add(user1);
        await _dbContext.SaveChangesAsync();

        var userController = new UserController(_dbContext);

        // Act
        var actionResult = await userController.Delete(2);
        var result = actionResult as NotFoundResult;

        // Assert
        Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
    }
}