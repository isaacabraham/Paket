namespace Paket.Refactor

open Paket
open Paket.NugetSources
open Paket.SemVer
open System

type GitHubSource = 
    { Owner : string
      Project : string
      Commit : string option }

type GitHubProjectStructure = 
    | PaketFiles
    | CustomFolder of Name : string
    | ProjectRoot

type GitHubDependency = 
    | File of Source : GitHubSource * File : string * ProjectStructure : GitHubProjectStructure
    | Repository of Source : GitHubSource

type PackageId = 
    | PackageId of string

/// Different types of dependencies.
type Dependency = 
    | GitHub of GitHubDependency
    | NuGet of PackageId * VersionRequirement option * ResolverStrategy

/// How project dependencies are referenced.
type ProjectDependencyReferenceKind = 
    /// Direct and indirect dependencies are referenced.
    | Loose
    /// Only direct dependencies are referenced.
    | Strict

/// Whether content files are allowed or not.
type ContentKind = 
    | Allowed
    | Ignored

/// Options regarding dependency resolution.
type DependencySetOptions = 
    { ContentRestrictions : ContentKind
      ProjectDependencyKind : ProjectDependencyReferenceKind }

/// The set of dependencies as specified in the .dependencies file.
type DependencySet = 
    { Options : DependencySetOptions
      NugetSources : NugetSource list
      Dependencies : Dependency list }

module private Sample = 
    //source https://nuget.org/api/v2
    //
    //nuget Newtonsoft.Json
    //nuget UnionArgParser
    //nuget NUnit.Runners >= 2.6
    //nuget NUnit >= 2.6
    //nuget FAKE
    //nuget FSharp.Formatting
    //nuget DotNetZip ~> 1.9.3
    //nuget SourceLink.Fake
    //nuget NuGet.CommandLine
    //nuget FSharp.Core.Microsoft.Signed
    //
    //github forki/FsUnit FsUnit.fs
    //github fsharp/FAKE modules/Octokit/Octokit.fsx    
    let dependencySet = 
        { Options = 
              { ContentRestrictions = Allowed
                ProjectDependencyKind = Loose }
          NugetSources = 
              [ RemoteFeed { Url = "https://nuget.org/api/v2"
                             Authentication = None } ]
          Dependencies = 
              [ NuGet(PackageId "Newtonsoft.Json", None, ResolverStrategy.Max)
                NuGet(PackageId "UnionArgParser", None, ResolverStrategy.Max)
                NuGet(PackageId "NUnit.Runners", Some <| VersionRequirement(VersionRange.AtLeast("2.6"), PreReleaseStatus.No), ResolverStrategy.Max)
                NuGet(PackageId "NUnit", Some <| VersionRequirement(VersionRange.AtLeast("2.6"), PreReleaseStatus.No), ResolverStrategy.Max)
                NuGet(PackageId "FAKE", None, ResolverStrategy.Max)
                NuGet(PackageId "FSharp.Formatting", None, ResolverStrategy.Max)
                NuGet(PackageId "DotNetZip", Some <| VersionRequirement(VersionRange.Between("1.9.3", "2.0.0"), PreReleaseStatus.No), ResolverStrategy.Max)
                NuGet(PackageId "SourceLink.Fake", None, ResolverStrategy.Max)
                NuGet(PackageId "NuGet.CommandLine", None, ResolverStrategy.Max)
                NuGet(PackageId "FSharp.Core.Microsoft.Signed", None, ResolverStrategy.Max)                
                GitHub(File({ Owner = "forki"; Project = "FsUnit"; Commit = None }, "FsUnit.fs", PaketFiles))
                GitHub(File({ Owner = "fsharp"; Project = "FAKE"; Commit = None }, "modules/Octokit/Octokit.fsx", PaketFiles)) ] }
