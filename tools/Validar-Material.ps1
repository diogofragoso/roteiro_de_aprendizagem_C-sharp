param([string]$Dotnet = 'dotnet', [switch]$SomenteDocumentacao)
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
Set-Location $repo
$falhas = New-Object 'System.Collections.Generic.List[string]'
$atividades = [IO.File]::ReadAllText((Join-Path $repo 'ATIVIDADES-POR-CONCEITO.md'))
$ids = @([regex]::Matches($atividades, '(?m)^\| (A\d{2}\.\d{2}) \|') | ForEach-Object { $_.Groups[1].Value })
if (($ids | Select-Object -Unique).Count -ne $ids.Count) { $falhas.Add('IDs duplicados') }
foreach ($n in 1..24) {
    $num = '{0:D2}' -f $n
    if (-not ($ids | Where-Object { $_.StartsWith("A$num.") })) { $falhas.Add("Aula $num sem atividades") }
    if (-not $atividades.Contains('id="aula-' + $num + '"')) { $falhas.Add("Ancora ausente $num") }
    $capitulo = @(Get-ChildItem -File -Filter "$num-*.md")
    if ($capitulo.Count -ne 1) { $falhas.Add("Quantidade de capitulos $num invalida"); continue }
    $texto = [IO.File]::ReadAllText($capitulo[0].FullName)
    $tempos = [regex]::Matches($texto, '(?m)^\| [^|\r\n]+ \| (\d+) \|')
    $soma = ($tempos | ForEach-Object { [int]$_.Groups[1].Value } | Measure-Object -Sum).Sum
    if ($soma -ne 210) { $falhas.Add("Aula $num soma $soma minutos") }
}
$documentos = @(Get-ChildItem -File -Filter '*.md') + @(Get-ChildItem ensino -Recurse -File -Filter '*.md') + @(Get-Item src/README.md)
foreach ($arquivo in $documentos) {
    $texto = [IO.File]::ReadAllText($arquivo.FullName)
    if ($texto.Contains([char]0xFFFD)) { $falhas.Add("UTF-8 invalido: $($arquivo.Name)") }
    if ([regex]::Matches($texto, '(?m)^```').Count % 2) { $falhas.Add("Bloco aberto: $($arquivo.Name)") }
    $colunas = 0
    foreach ($linha in $texto -split '\r?\n') {
        if ($linha.StartsWith('|')) {
            $quantidade = [regex]::Matches($linha, '(?<!\\)\|').Count
            if ($colunas -eq 0) { $colunas = $quantidade }
            elseif ($colunas -ne $quantidade) { $falhas.Add("Tabela inconsistente: $($arquivo.Name): $linha") }
        } else { $colunas = 0 }
    }
    foreach ($link in [regex]::Matches($texto, '\]\(([^)]+)\)')) {
        $destino = $link.Groups[1].Value
        if ($destino -match '^(https?://|#)') { continue }
        $partes = $destino -split '#', 2
        $local = Join-Path $arquivo.DirectoryName $partes[0]
        if (-not (Test-Path -LiteralPath $local)) { $falhas.Add("Link ausente: $($arquivo.Name): $destino"); continue }
        if ($partes.Count -eq 2 -and $partes[1] -match '^aula-\d{2}$') {
            if (-not [IO.File]::ReadAllText($local).Contains('id="' + $partes[1] + '"')) { $falhas.Add("Ancora ausente: $destino") }
        }
    }
}
if ($falhas.Count) { throw ($falhas -join "`n") }
Write-Output "Documentacao: $($ids.Count) atividades unicas, 24 aulas, tempos, links e blocos conferidos."
if ($SomenteDocumentacao) { return }
$executavel = (Get-Command $Dotnet -ErrorAction Stop).Source
function Dotnet([string[]]$Argumentos) {
    & $executavel @Argumentos
    if ($LASTEXITCODE -ne 0) { throw "dotnet falhou ($LASTEXITCODE): $($Argumentos -join ' ')" }
}
$destino = 'artifacts/checkpoints-' + [Guid]::NewGuid().ToString('N').Substring(0,8)
& (Join-Path $PSScriptRoot 'Gerar-Checkpoints.ps1') -Destino $destino
foreach ($n in 0..20) {
    $num = '{0:D2}' -f $n
    $projeto = "$destino/$num/SupermercadoApp.csproj"
    Dotnet @('build', $projeto, '-c', 'Release', '--nologo', '-v', 'quiet', '-warnaserror')
    if ($n -le 4) { Dotnet @('run','--project',$projeto,'-c','Release','--no-build','--','--verificar') }
}
Dotnet @('build','Supermercado.sln','-c','Release','--nologo','-warnaserror')
Dotnet @('run','--project','tests/SupermercadoApp.Verificacoes','-c','Release','--no-build')
Dotnet @('run','--project',"$destino/20/tests/Verificacoes/Verificacoes.csproj",'-c','Release')
$relatorio = "Data: $([DateTime]::UtcNow.ToString('u'))`nSDK: $(& $executavel --version)`nDocumentacao: $($ids.Count) atividades, 24 aulas.`n21 checkpoints compilados; consoles 00-04 executados.`nSolucao final e verificacoes com fakes executadas, incluindo checkpoint 20.`nPostgreSQL, cliques e rede: nao executados por este script.`n"
[IO.File]::WriteAllText((Join-Path $repo "$destino/VALIDACAO.txt"), $relatorio, (New-Object Text.UTF8Encoding($false)))
Write-Output "Validacao concluida; relatorio em $destino/VALIDACAO.txt"
