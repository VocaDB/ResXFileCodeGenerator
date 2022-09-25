using FluentAssertions;
using Xunit;
using static System.Guid;

namespace VocaDb.ResXFileCodeGenerator.Tests;

public class GroupResxFilesTests
{
	[Fact]
	public void CompareGroupedAdditionalFile_SameRoot_SameSubFiles_DifferentOrder()
	{
		var v1 = new GroupedAdditionalFile(new AdditionalTextWithHash(new AdditionalTextStub(
			@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.resx"), Parse("47FFD75C-3254-4851-8E1C-CBDDCDCE1D9B")),
		new[]
		{
			new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.da.resx"), Parse("B7EDA261-6923-4526-AFB7-B2A64984F099")),
			new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.vi.resx"), Parse("5B2BA95C-FB9C-47C5-9C03-280B63D8DD27")),
		});

		var v2 = new GroupedAdditionalFile(new AdditionalTextWithHash(new AdditionalTextStub(
				@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.resx"), Parse("47FFD75C-3254-4851-8E1C-CBDDCDCE1D9B")),
			new[]
			{

				new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.vi.resx"), Parse("5B2BA95C-FB9C-47C5-9C03-280B63D8DD27")),
				new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.da.resx"), Parse("B7EDA261-6923-4526-AFB7-B2A64984F099")),
			}
		);
		v1.Should().Be(v2);
	}

	[Fact]
	public void CompareGroupedAdditionalFile_SameRoot_DiffSubFilesNames()
	{
		var v1 = new GroupedAdditionalFile(new AdditionalTextWithHash(new AdditionalTextStub(
				@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.resx"), Parse("47FFD75C-3254-4851-8E1C-CBDDCDCE1D9B")),
			new[]
			{
				new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.en.resx"), Parse("B7EDA261-6923-4526-AFB7-B2A64984F099")),
				new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.fr.resx"), Parse("5B2BA95C-FB9C-47C5-9C03-280B63D8DD27")),
			});

		var v2 = new GroupedAdditionalFile(new AdditionalTextWithHash(new AdditionalTextStub(
				@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.resx"), Parse("47FFD75C-3254-4851-8E1C-CBDDCDCE1D9B")),
			new[]
			{

				new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.de.resx"), Parse("5B2BA95C-FB9C-47C5-9C03-280B63D8DD27")),
				new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.ro.resx"), Parse("B7EDA261-6923-4526-AFB7-B2A64984F099")),
			}
		);
		v1.Should().NotBe(v2);
	}

	[Fact]
	public void CompareGroupedAdditionalFile_SameRoot_DiffSubFileContent()
	{
		var v1 = new GroupedAdditionalFile(new AdditionalTextWithHash(new AdditionalTextStub(
				@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.resx"), Parse("47FFD75C-3254-4851-8E1C-CBDDCDCE1D9B")),
			new[]
			{
				new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.da.resx"), Parse("771F9C76-D9F4-4AF4-95D2-B3426F9EC15A")),
				new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.vi.resx"), Parse("5B2BA95C-FB9C-47C5-9C03-280B63D8DD27")),
			});

		var v2 = new GroupedAdditionalFile(new AdditionalTextWithHash(new AdditionalTextStub(
				@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.resx"), Parse("47FFD75C-3254-4851-8E1C-CBDDCDCE1D9B")),
			new[]
			{

				new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.vi.resx"), Parse("5B2BA95C-FB9C-47C5-9C03-280B63D8DD27")),
				new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.da.resx"), Parse("B7EDA261-6923-4526-AFB7-B2A64984F099")),
			}
		);
		v1.Should().NotBe(v2);
	}

	[Fact]
	public void CompareGroupedAdditionalFile_DiffRootContent_SameSubFiles()
	{
		var v1 = new GroupedAdditionalFile(new AdditionalTextWithHash(new AdditionalTextStub(
				@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.resx"), Parse("47FFD75C-3254-4851-8E1C-CBDDCDCE1D9B")),
			new[]
			{
				new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.da.resx"), Parse("B7EDA261-6923-4526-AFB7-B2A64984F099")),
				new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.vi.resx"), Parse("5B2BA95C-FB9C-47C5-9C03-280B63D8DD27")),
			});

		var v2 = new GroupedAdditionalFile(new AdditionalTextWithHash(new AdditionalTextStub(
				@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.resx"), Parse("A7E92264-8047-4668-979F-6EFC14EBAFC5")),
			new[]
			{

				new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.vi.resx"), Parse("5B2BA95C-FB9C-47C5-9C03-280B63D8DD27")),
				new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.da.resx"), Parse("B7EDA261-6923-4526-AFB7-B2A64984F099")),
			}
		);
		v1.Should().NotBe(v2);
	}

	static readonly (string Path, Guid Hash)[] s_data =
	{
		(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.da.resx", Parse("00000000-0000-0000-0000-000000000001")),
		(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.resx", Parse("00000000-0000-0000-0000-000000000002")),
		(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.vi.resx", Parse("00000000-0000-0000-0000-000000000003")),
		(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgLive.da.resx", Parse("00000000-0000-0000-0000-000000000004")),
		(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgLive.resx", Parse("00000000-0000-0000-0000-000000000005")),
		(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgLive.vi.resx", Parse("00000000-0000-0000-0000-000000000006")),
		(@"D:\src\xhg\y\Areas\Identity\Pages\Login.da.resx", Parse("00000000-0000-0000-0000-000000000007")),
		(@"D:\src\xhg\y\Areas\Identity\Pages\Login.resx", Parse("00000000-0000-0000-0000-000000000008")),
		(@"D:\src\xhg\y\Areas\Identity\Pages\Login.vi.resx", Parse("00000000-0000-0000-0000-000000000009")),
		(@"D:\src\xhg\y\Areas\QxModule\Pages\QasdLogon.da.resx", Parse("00000000-0000-0000-0000-000000000010")),
		(@"D:\src\xhg\y\Areas\QxModule\Pages\QasdLogon.resx", Parse("00000000-0000-0000-0000-000000000011")),
		(@"D:\src\xhg\y\Areas\QxModule\Pages\QasdLogon.vi.resx", Parse("00000000-0000-0000-0000-000000000012")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.cs-cz.resx", Parse("00000000-0000-0000-0000-000000000013")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.da.resx", Parse("00000000-0000-0000-0000-000000000014")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.de.resx", Parse("00000000-0000-0000-0000-000000000015")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.es.resx", Parse("00000000-0000-0000-0000-000000000016")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.fi.resx", Parse("00000000-0000-0000-0000-000000000017")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.fr.resx", Parse("00000000-0000-0000-0000-000000000018")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.it.resx", Parse("00000000-0000-0000-0000-000000000019")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.lt.resx", Parse("00000000-0000-0000-0000-000000000020")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.lv.resx", Parse("00000000-0000-0000-0000-000000000021")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.nb-no.resx", Parse("00000000-0000-0000-0000-000000000022")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.nl.resx", Parse("00000000-0000-0000-0000-000000000023")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.nn-no.resx", Parse("00000000-0000-0000-0000-000000000024")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.pl.resx", Parse("00000000-0000-0000-0000-000000000025")),
		( @"D:\src\xhg\y\Areas\QxModule\QtrController.resx", Parse("00000000-0000-0000-0000-000000000026")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.ru.resx", Parse("00000000-0000-0000-0000-000000000027")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.sv.resx", Parse("00000000-0000-0000-0000-000000000028")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.tr.resx", Parse("00000000-0000-0000-0000-000000000029")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.vi.resx", Parse("00000000-0000-0000-0000-000000000030")),
		(@"D:\src\xhg\y\Areas\QxModule\QtrController.zh-cn.resx", Parse("00000000-0000-0000-0000-000000000031")),
		(@"D:\src\xhg\y\DataAnnotations\DataAnnotation.da.resx", Parse("00000000-0000-0000-0000-000000000032")),
		(@"D:\src\xhg\y\DataAnnotations\DataAnnotation.resx", Parse("00000000-0000-0000-0000-000000000033")),
		(@"D:\src\xhg\y\DataAnnotations\DataAnnotation2.resx", Parse("00000000-0000-0000-0000-000000000034")),
	};

	[Fact]
	public void FileGrouping()
	{
		var result = GroupResxFiles.Group(s_data.Select(x => new AdditionalTextWithHash(new AdditionalTextStub(x.Path), x.Hash)).OrderBy(x => NewGuid()).ToArray());

		var testData = new List<GroupedAdditionalFile>
		{
			new GroupedAdditionalFile(new AdditionalTextWithHash(new AdditionalTextStub(
					@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.resx"), Parse("00000000-0000-0000-0000-000000000002")),
				new[]
				{
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.da.resx"), Parse("00000000-0000-0000-0000-000000000001")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.vi.resx"), Parse("00000000-0000-0000-0000-000000000003")),
				}
			),
			new GroupedAdditionalFile(new AdditionalTextWithHash(new AdditionalTextStub(
					@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgLive.resx"), Parse("00000000-0000-0000-0000-000000000005")),
				new[]
				{
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgLive.da.resx"), Parse("00000000-0000-0000-0000-000000000004")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgLive.vi.resx"), Parse("00000000-0000-0000-0000-000000000006")),
				}
			),
			new GroupedAdditionalFile(new AdditionalTextWithHash(new AdditionalTextStub(
					@"D:\src\xhg\y\Areas\Identity\Pages\Login.resx"), Parse("00000000-0000-0000-0000-000000000008")),
				new[]
				{
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\Identity\Pages\Login.da.resx"), Parse("00000000-0000-0000-0000-000000000007")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\Identity\Pages\Login.vi.resx"), Parse("00000000-0000-0000-0000-000000000009")),
				}
			),
			new GroupedAdditionalFile(new AdditionalTextWithHash(new AdditionalTextStub(
					@"D:\src\xhg\y\Areas\QxModule\Pages\QasdLogon.resx"), Parse("00000000-0000-0000-0000-000000000011")),
				new[]
				{
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\Pages\QasdLogon.da.resx"), Parse("00000000-0000-0000-0000-000000000010")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\Pages\QasdLogon.vi.resx"), Parse("00000000-0000-0000-0000-000000000012")),
				}
			),
			new GroupedAdditionalFile(new AdditionalTextWithHash(new AdditionalTextStub(
					@"D:\src\xhg\y\Areas\QxModule\QtrController.resx"), Parse("00000000-0000-0000-0000-000000000026")),
				new[]
				{
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.cs-cz.resx"), Parse("00000000-0000-0000-0000-000000000013")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.da.resx"), Parse("00000000-0000-0000-0000-000000000014")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.de.resx"), Parse("00000000-0000-0000-0000-000000000015")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.es.resx"), Parse("00000000-0000-0000-0000-000000000016")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.fi.resx"), Parse("00000000-0000-0000-0000-000000000017")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.fr.resx"), Parse("00000000-0000-0000-0000-000000000018")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.it.resx"), Parse("00000000-0000-0000-0000-000000000019")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.lt.resx"), Parse("00000000-0000-0000-0000-000000000020")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.lv.resx"), Parse("00000000-0000-0000-0000-000000000021")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.nb-no.resx"), Parse("00000000-0000-0000-0000-000000000022")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.nl.resx"), Parse("00000000-0000-0000-0000-000000000023")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.nn-no.resx"), Parse("00000000-0000-0000-0000-000000000024")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.pl.resx"), Parse("00000000-0000-0000-0000-000000000025")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.ru.resx"), Parse("00000000-0000-0000-0000-000000000027")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.sv.resx"), Parse("00000000-0000-0000-0000-000000000028")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.tr.resx"), Parse("00000000-0000-0000-0000-000000000029")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.vi.resx"), Parse("00000000-0000-0000-0000-000000000030")),
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.zh-cn.resx"), Parse("00000000-0000-0000-0000-000000000031")),
				}),
			new GroupedAdditionalFile(new AdditionalTextWithHash(new AdditionalTextStub(
					@"D:\src\xhg\y\DataAnnotations\DataAnnotation.resx"), Parse("00000000-0000-0000-0000-000000000033")),
				new[]
				{
					new AdditionalTextWithHash(new AdditionalTextStub(@"D:\src\xhg\y\DataAnnotations\DataAnnotation.da.resx"), Parse("00000000-0000-0000-0000-000000000032"))
				}
			),
			new GroupedAdditionalFile(new AdditionalTextWithHash(new AdditionalTextStub(
					@"D:\src\xhg\y\DataAnnotations\DataAnnotation2.resx"), Parse("00000000-0000-0000-0000-000000000034")),
				Array.Empty<AdditionalTextWithHash>()
			)
		};
		var resAsList = result.ToList();
		resAsList.Count.Should().Be(testData.Count);
		foreach (var groupedAdditionalFile in testData)
		{
			resAsList.Should().Contain(groupedAdditionalFile);
		}
	}

	[Fact]
	public void ResxGrouping()
	{
		var result = GroupResxFiles.DetectChildCombos(GroupResxFiles.Group(s_data.Select(x => new AdditionalTextWithHash(new AdditionalTextStub(x.Path), NewGuid())).OrderBy(x => NewGuid()).ToArray()).ToArray()).ToList();
		var expected = new List<CultureInfoCombo>
		{
			new CultureInfoCombo(new[]
			{
				new AdditionalTextWithHash(new AdditionalTextStub("test.da.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.vi.resx"), NewGuid())
			}),
			new CultureInfoCombo(new[]{ new AdditionalTextWithHash(new AdditionalTextStub("test.da.resx"), NewGuid())}),
			new CultureInfoCombo(Array.Empty<AdditionalTextWithHash>()),
			new CultureInfoCombo(new[]
			{
				new AdditionalTextWithHash(new AdditionalTextStub("test.cs-cz.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.da.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.de.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.es.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.fi.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.fr.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.it.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.lt.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.lv.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.nb-no.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.nl.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.nn-no.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.pl.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.ru.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.sv.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.tr.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.vi.resx"), NewGuid()),
				new AdditionalTextWithHash(new AdditionalTextStub("test.zh-cn.resx"), NewGuid()),
			}),
		};
		result.Count.Should().Be(expected.Count);
		result.Should().BeEquivalentTo(expected);
	}
}


