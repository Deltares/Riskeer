C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe DeltaShell.sln /t:clean /m /nr:true /v:m
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe DeltaShell.sln /t:build /l:MSBuildProfileFileLogger,.\build\tools\MSBuildProfiler\MSBuildProfiler.dll
.\build\tools\MSBuildProfiler\MSBuildProfilerGui build.snapshot