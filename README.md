![App image](https://raw.githubusercontent.com/AxlRocket/YT-DLP-GuiWrapper/refs/heads/main/app.png)

# YT-DLP-GuiWrapper
I was borred to use command line to download songs so I made a simple app that hand all for me

> [!IMPORTANT]  
> __ONLY FOR YOUTUBE AND YOUTUBE MUSIC__

> [!TIP]
> Use [MusicBrainz](https://musicbrainz.org/) to add all metadatas to your files
  
## How to use it

First you need :  
- yt-dlp
- ffmpeg
- MacOS (at least 13 - Ventura) __or__ Windows 10 x64 __or__ Windows 11 x64

Copy the app in your Application folder, open a terminal and copy/paste this :
```
codesign --deep --force --sign - /Applications/YT-DLP-GuiWrapper.app
```
<sub>I can't sign the app without an Apple Developper account</sub>

Open the app and paste your link, select the file output format and the bitrate, click on the Download button and that's all.
If you're downloading a playlist the app will create a folder otherwise the file will be downloaded directly

> [!NOTE]
> Find the downloaded files in your __Downloads__ folder

> [!NOTE]
> Output bitrate is ignored if you download in FLAC or WAV format

## Install yt-dlp & ffmpeg

### Windows

With chocolatey :
```
choco install yt-dlp
```
```
choco install ffmpeg
```

With winget :
```
winget install yt-dlp
```
```
winget install ffmpeg
```

Manually : (you have to put yt-dlp and ffmpeg in specifics directories)  
For yt-dlp :
> C:\Program Files\yt-dlp\yt-dlp.exe

For ffmpeg:
> C:\Program Files\ffmpeg\bin\ffmpeg.exe

### MacOS

You need [Homebrew](https://brew.sh/)

```
brew install yt-dlp
```
```
brew install ffmpeg
```

## Tests

App tested on MacOS Sequoia (15), MacOS Ventura (13) and Windows 11 x64

With yt-dlp 2026.03.17 and ffmpeg 8.1

## About

Coded in C# (NET10) with Avalonia UI 
