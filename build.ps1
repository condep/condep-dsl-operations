Import-Module .\tools\psake.psm1Invoke-Psake .\build\default.ps1if (!($psake.build_success)) {    throw "Error when building"}