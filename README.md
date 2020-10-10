# Pipelines NET

[![NuGet](https://img.shields.io/nuget/v/Pipelines.Net.svg?style=plastic)](https://www.nuget.org/packages/Pipelines.Net/)

Do you use this library? [Fill in the form](https://forms.gle/Y8aogzwPQ3sLKXMCA) and let us know what you like.

Pipelines NET is a library developed for purpose of composing logical blocks and actions into algorythm. Developing this library, next goals were claimed:

- Create a self-descriptive single-action processor
```cs
public class CreateTitle { }
public class CreateDescriptionBlock { }
public class HighlightSearchTextInDescription { }
```
- Possibility to combine processors together so they make up a bigger logical algorythm

```ts
public class ComposeNewsBlock 
{
    public IEnumerable GetProcessors() 
    {
        yield return CreateTitle;
        
        yield return CreateDescriptionBlock;
        
        yield return HighlightSearchTextInDescription;
    }
}
```
- It should be possible to extend algorythm (_i.e. in the sample above I would like to add crop possibility, capitilize logic and logging mechanism to check state of the news block after each processor_)

## About the library

### Abstractions

The main abstractions of the library represent two interfaces, which are: `IProcessor` and `IPipeline`.

- **`IProcessor`** - is an interface representing a unit which can process some information. You can think of it like about actions in command pattern. It defines and responsible only for a single method: `Task Execute(object args)`, which means that whatever information is passed, it will be somehow processed.
- **`IPipeline`** - binds together logical processors and itself represents a complete action instruction, which defines how processors must be executed.

### Helpful classes

When abstractions are defined, it would be great to see them in action. For this purpose were created several useful classes: `PipelineRunner`, `PipelineContext`, `SafeProcessor`.

- **`PipelineRunner`** - simply runs your pipeline, a set of processors or a single processor.
- **`PipelineContext`** - introduces possibility to keep context information about the flow of the pipeline, like: messages and whether pipeline was aborted.
- **`SafeProcessor`** - implementation of the `IProcessor`, which has a condition, that checks parameters before they will be passed to execution.

### Documentation

Currently code examples can be found on [Witty Lion's official blog](https://wittylion.github.io/).

The articles on the site show how to:
- [Easy create and use context](https://wittylion.github.io/2019/01/25/ways-to-create-context.html)
- [Use a new namespace based pipeline](https://wittylion.github.io/2019/07/13/namespace-based-pipeline.html)
- [Use a new auto processor](https://wittylion.github.io/2020/10/04/auto-processor.html)
- [Modify pipelines](https://wittylion.github.io/2020/08/05/modify-pipeline.html)
- [Use common processors like "Transform Property"](https://wittylion.github.io/2019/01/23/transform-property-processor.html)

Project example: [https://github.com/SergAtGitHub/Solo.BinaryTree](https://github.com/SergAtGitHub/Solo.BinaryTree)

If you want to learn more about any topic of this library, send a message to [wittylioncompany@gmail.com](mailto:wittylioncompany@gmail.com) or [fill in the form](https://forms.gle/Y8aogzwPQ3sLKXMCA).

## Installation and Usage

```ps
PM> Install-Package Pipelines.Net
```

To make it easier to use this mechanism a [yeoman generator](https://www.npmjs.com/package/generator-chain) was created, which can help you easily create common files, used with this library.