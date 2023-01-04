using System.Globalization;
using FluentAssertions;
using Xunit;

namespace VocaDb.ResXFileCodeGenerator.Tests.IntegrationTests;

public class TestResxFiles
{
	[Fact]
	public void TestNormalResourceGen()
	{
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("da");
		Test1.CreateDate.Should().Be("OldestDa");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
		Test1.CreateDate.Should().Be("Oldest");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("ch");
		Test1.CreateDate.Should().Be("Oldest");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
		Test1.CreateDate.Should().Be("OldestEnUs");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("da-DK");
		Test1.CreateDate.Should().Be("OldestDaDK");
	}
	[Fact]
	public void TestCodeGenResourceGen()
	{
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("da");
		Test2.CreateDate.Should().Be("OldestDa");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
		Test2.CreateDate.Should().Be("Oldest");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("ch");
		Test2.CreateDate.Should().Be("Oldest");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
		Test2.CreateDate.Should().Be("OldestEnUs");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("da-DK");
		Test2.CreateDate.Should().Be("OldestDaDK");
	}


	[Fact]
	public void TestCodeGenOnStaticMethods()
	{
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
		Test3.HttpError("404","Not Found","Order 123").Should().Be("ErrorCode:404 , ErrorDesc:Not Found , ResourceId:Order 123");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("da-dk");
		Test3.HttpError("404", "Not Found", "Order 123").Should().Be("ErrorCodeDaDk:404 , ErrorDescDaDk:Not Found , ResourceIdDaDk:Order 123");
	}

}
