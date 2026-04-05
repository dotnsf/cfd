# CFD ビルドスクリプト
# 単一実行ファイル (cfd.exe) を生成します

Write-Host "CFD をビルドしています..." -ForegroundColor Cyan

# プロジェクトディレクトリに移動
Set-Location -Path "$PSScriptRoot/CfdApp"

# クリーンビルド
Write-Host "クリーンアップ中..." -ForegroundColor Yellow
dotnet clean -c Release

# パブリッシュ（単一実行ファイル）
Write-Host "パブリッシュ中..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nビルド成功!" -ForegroundColor Green
    Write-Host "実行ファイル: $PSScriptRoot\CfdApp\bin\Release\net8.0\win-x64\publish\cfd.exe" -ForegroundColor Green
    
    # ファイルサイズを表示
    $exePath = "$PSScriptRoot\CfdApp\bin\Release\net8.0\win-x64\publish\cfd.exe"
    if (Test-Path $exePath) {
        $fileSize = (Get-Item $exePath).Length / 1MB
        Write-Host "ファイルサイズ: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Cyan
    }
} else {
    Write-Host "`nビルド失敗" -ForegroundColor Red
    exit 1
}

# 元のディレクトリに戻る
Set-Location -Path $PSScriptRoot

# Made with Bob
