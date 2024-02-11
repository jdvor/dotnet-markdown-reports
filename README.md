# mdreport

Command line utility for transforming various .NET build or test XML reports into human-readable markdown format.

The motivation and main purpose for this tool was to have an easy and **consistent** way how to produce 
markdown summary report during project build within GitHub CI pipeline.

Implemented:

* TRX test reports
* Cobertura code coverage reports
* Roslynator code analysis report


## Installation

```shell
dotnet tool install mdreport --global
```

## Example usage

### TRX test reports

*TRX test reports written directly to stdout:*
```shell
# a particular trx report
mdreport trx "some/dir/sub/whatever.trx"

# every trx report from a directory
mdreport trx "some/dir/sub"

# find all trx reports using globbing pattern
mdreport trx "some/**/*.trx"
```

*TRX test reports written into a file:*
```shell
mdreport trx "{path-to-a-file-or-directory-or-glob-pattern}" -o "{output-file-path}"
```

### Cobertura code coverage reports

*Cobertura code coverage reports directly to stdout:*
```shell
# a particular cobertura report
mdreport cobertura "some/dir/1c2faf42-e31c-4dc9-bb68-92ed2a085db4/cobertura.coverage.xml"

# every cobertura report from a directory (finds recursively all 'cobertura.coverage.xml' files)
mdreport cobertura "some/dir/sub"

# find all cobertura reports using globbing pattern
mdreport cobertura "some/dir/**/cobertura.coverage.xml"
```

*Cobertura test reports written into a file:*
```shell
mdreport cobertura "{path-to-a-file-or-directory-or-glob-pattern}" -o "{output-file-path}"
```

### Roslynator report

*Roslynator test reports directly to stdout:*
```shell
mdreport roslynator "some/dir/sub/roslynator.xml"
```

*Roslynator test reports written into a file:*
```shell
mdreport roslynator "some/dir/sub/roslynator.xml" -o "{output-file-path}"
```

> Unlike previous reports - trx or cobertura, where it is expected that there are multiple reports from a build 
> or test run (usually one per project in the solution), the roslynator produces single XML file for a build. 
> So if multiple roslynator.xml files are found when using directory or globbing pattern sources, they are not merged, 
> but the most recent one is picked as a source. 
