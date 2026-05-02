![App image](https://raw.githubusercontent.com/AxlRocket/YT-DLP-GuiWrapper/refs/heads/main/app.png)

# YT-DLP-GuiWrapper
I was borred to use command line to download songs so I made a simple app that hand all for me

> [!IMPORTANT]  
> __ONLY FOR YOUTUBE AND YOUTUBE MUSIC__

## How to use it

First you need :  
- yt-dlp
- ffmpeg
- MacOS 13 (Ventura) or Windows 10 x64

Open the app and paste your link, select the file output format and the bitrate, click on the Download button and that's all.
If you're downloading a playlist the app will create a folder otherwise the file will be downloaded directly

> Find the downloaded files in your Downloads folder

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

## 
