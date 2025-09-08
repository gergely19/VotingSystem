using Bunit.TestDoubles;
using Bunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.DataAccess.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VotingSystem.Blazor.WebAssembly.Layout;
using Blazored.LocalStorage;
using VotingSystem.Blazor.WebAssembly.Services;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Components.Routing;

namespace VotingSystem.Tests.ComponentTests;

public class BlazorMenuComponentsTests : IDisposable
{
    private readonly TestContext _context = new();
    private readonly Mock<IAuthenticationService> _authServiceMock = new();
    private readonly FakeNavigationManager _navManager;

    public BlazorMenuComponentsTests()
    {
        _authServiceMock
                    .Setup(x => x.GetCurrentlyLoggedInUserAsync())
                    .ReturnsAsync("admin");

        _authServiceMock
            .Setup(x => x.LogoutAsync())
            .Returns(Task.CompletedTask);

        _context.Services.AddSingleton<IAuthenticationService>(_authServiceMock.Object);
        _navManager = _context.Services.GetRequiredService<FakeNavigationManager>();
    }

    public void Dispose()
    {
        _context.Dispose();
    }


    [Fact]
    public void MenuComponent_ShouldRenderWithUserInfo()
    {
        // Act
        var cut = _context.RenderComponent<MenuComponent>();

        // Assert
        var brand = cut.Find(".navbar-brand");
        Assert.Equal("Anonim szavazó rendszer", brand.TextContent);

        var welcome = cut.Find(".navbar-text");
        Assert.Equal("Üdvözlünk admin!", welcome.TextContent);

        var logoutButton = cut.Find("button[data-testid='logout']");
        Assert.Equal("Kijelentkezés", logoutButton.TextContent.Trim());
    }

    [Fact]
    public void MenuComponent_ShouldContainNavLinks()
    {
        // Act
        var cut = _context.RenderComponent<MenuComponent>();

        var navLinks = cut.FindAll("a.nav-link").Select(x => x.TextContent.Trim()).ToList();

        // Assert
        Assert.Contains("Szavazásaim", navLinks);
        Assert.Contains("Új szavazás", navLinks);
    }

    [Fact]
    public void Logout_Click_ShouldCallLogoutAndNavigate()
    {
        // Arrange
        var cut = _context.RenderComponent<MenuComponent>();
        var logoutButton = cut.Find("button[data-testid='logout']");

        // Act
        logoutButton.Click();

        // Assert
        _authServiceMock.Verify(x => x.LogoutAsync(), Times.Once);
        Assert.Contains("/login", _navManager.Uri);
    }

    [Fact]
    public void MenuComponent_WhenUserIsNotAuthenticated_ShouldNotShowWelcome()
    {
        // Arrange - új context, új mock más viselkedéssel
        var context = new TestContext();
        var mockAuth = new Mock<IAuthenticationService>();
        mockAuth.Setup(x => x.GetCurrentlyLoggedInUserAsync()).ReturnsAsync((string?)null);
        context.Services.AddSingleton<IAuthenticationService>(mockAuth.Object);
        context.Services.AddSingleton<FakeNavigationManager>();

        // Act
        var cut = context.RenderComponent<MenuComponent>();

        // Assert
        Assert.Empty(cut.FindAll(".navbar-text"));
        Assert.Empty(cut.FindAll("button[data-testid='logout']"));

        context.Dispose();
    }


}








