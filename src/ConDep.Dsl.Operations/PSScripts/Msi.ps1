function Install-ConDepMsiFromUri($uri, $tempFilePath) {
    $tempFilePath = $ExecutionContext.InvokeCommand.ExpandString($tempFilePath)
	write-Host "Downloading Msi from $uri"
	Invoke-WebRequest $uri -OutFile $tempFilePath 
	write-Host "Download finished"
	write-Host "Installing"
	InstallMsi $tempFilePath
}

function Install-ConDepMsiFromFile($path) {
    $path = $ExecutionContext.InvokeCommand.ExpandString($path)

	if(Test-Path $path) {
		InstallMsi $path
	}
	else {
		throw "$path not found."
	}
}

function InstallMsi($path) {
	cmd /c "msiexec /i $path /q /l* $($path).log"
	if($LASTEXITCODE > 0) {
		throw "Exit code $lastexitcode"
	}
}

function Install-ConDepExecutableFromFile($path, $params) {
    $path = $ExecutionContext.InvokeCommand.ExpandString($path)
	cmd /c "$path $params"
}

function Get-ConDepRemoteFile($uri, $tempFilePath) {
    $tempFilePath = $ExecutionContext.InvokeCommand.ExpandString($tempFilePath)
	write-Host "Downloading executable from $uri"
	Invoke-WebRequest $uri -OutFile $tempFilePath 
	write-Host "Download finished"
}