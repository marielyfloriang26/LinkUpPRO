$files = Get-ChildItem -Path C:\Users\orlan\Documents\GitHub\LinkUpPro\LinkUpPro.Presentation\Views -Filter "*.cshtml" -Recurse | Where-Object { $_.FullName -notmatch "_SidebarLeft.cshtml" }

foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw
    if ($content -match '<aside class="lup-sidebar lup-sidebar--left">[\s\S]*?</aside>') {
        $newContent = $content -replace '<aside class="lup-sidebar lup-sidebar--left">[\s\S]*?</aside>', '<partial name="_SidebarLeft" />'
        Set-Content -Path $file.FullName -Value $newContent
        Write-Host "Updated $($file.Name)"
    }
}
