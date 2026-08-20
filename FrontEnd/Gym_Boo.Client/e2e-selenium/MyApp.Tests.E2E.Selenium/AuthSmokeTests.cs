using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace MyApp.Tests.E2E.Selenium;

public class AuthSmokeTests : IDisposable
{
    private readonly IWebDriver _driver;

    public AuthSmokeTests()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--window-size=1280,720");

        _driver = new ChromeDriver(options);
    }

    [Fact]
    public void LoginPage_ShouldDisplaySignInForm()
    {
        _driver.Navigate().GoToUrl("http://localhost:5173/login");

        _driver.FindElement(By.TagName("h1")).Text.Should().Be("GYMBOO");
        _driver.FindElement(By.Id("email")).Displayed.Should().BeTrue();
        _driver.FindElement(By.Id("password")).Displayed.Should().BeTrue();
    }

    [Fact]
    public void LoginPage_ShouldNavigateToRegister()
    {
        _driver.Navigate().GoToUrl("http://localhost:5173/login");

        _driver.FindElement(By.XPath("//button[contains(., 'Sign up free')]")).Click();

        _driver.Url.Should().Contain("/register");
        _driver.FindElement(By.Id("name")).Displayed.Should().BeTrue();
        _driver.FindElement(By.Id("register-email")).Displayed.Should().BeTrue();
    }

    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}