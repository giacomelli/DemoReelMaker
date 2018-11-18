#!/bin/bash

###################################
# This script make a demo reel from a list of videos from youtube.
# 
# Usage:
# * Create a file list.txt and add the url of each video from youtube in one line (last line should be empty):
# https://youtu.be/ixCI4JXUuUE
# https://youtu.be/1Gr77r8Ahg0
# https://youtu.be/QhQ9NbuAOwI
# https://youtu.be/oh_Ur5OKhmQ
# 
# * Call the script passing the list file and the start and duration to cut videos do demo reel:
#   ./makeDemoReel.sh list.txt 00:00:10 00:00:25 
###################################

clearFolders() 
{
    log "Cleaning temp folder..."
    rm -rf temp
    mkdir temp
}

downloadVideos()
{
    log "Downwload videos from $listFile..."

    while IFS= read line
    do
        cd temp  
        log "Downwload: $line"
        youtube-dl --no-warnings $line
        cd ..
    done < "$listFile"
}

cutVideos()
{
    log "Cutting videos start from $cutFrom to $cutDuration...";
    cd temp
    videoCount=0;
    for i in *.*;
    do name=`echo $i | cut -d'.' -f1`;
        log "Cutting $i";
        videoCount=$(expr $videoCount + 1)
        ffmpeg -loglevel error -i "$i" -ss "$cutFrom" -t "$cutDuration" -async 1 "video_${videoCount}.mp4"

        log "Removing original video $i...";
        rm -f "$i"
    done
    cd ..
}

concatVideos()
{
    cd temp
    log "Concatenating videos..."
   
    for f in video_*; 
    do 
        echo "file '$f'" >> concat-list.txt; 
    done
    printf "file '%s'\n" video_* > concat-list.txt

    ## Builds the ffmpeg concat command to string.
    ## inputs.
    cmd='ffmpeg -loglevel error '
    for f in video_*; 
    do 
        cmd="${cmd}-i $f " 
    done

    # filter.
    cmd="$cmd-filter_complex \""

    videoIndex=0;
    videoLetter="a"
    for f in video_*; 
    do 
        cmd="${cmd}[$videoIndex]scale=1920x1080,setdar=16/9[$videoLetter];" 
        videoIndex=$(expr $videoIndex + 1)
        videoLetter=$(echo "$videoLetter" | tr "0-9a-z" "1-9a-z_")
    done

    videoIndex=0;
    videoLetter="a"
    for f in video_*; 
    do 
        cmd="${cmd}[$videoLetter][$videoIndex:a]" 
        videoIndex=$(expr $videoIndex + 1)
        videoLetter=$(echo "$videoLetter" | tr "0-9a-z" "1-9a-z_")
    done

    # concat.
    cmd="${cmd}concat=n=$videoIndex:v=1:a=1[v][a]\""
    cmd="${cmd} -map \"[v]\" -map \"[a]\""
    cmd="${cmd} concatenated-videos.mp4"

    ## Runs the ffmpeg concat command.
    eval $cmd;
    cd ..
}

addWatermark()
{
    log "Adding watermark..."
    
    cd temp
    ffmpeg -loglevel error -i concatenated-videos.mp4 -i ../watermarks/$watermark -filter_complex 'overlay=x=(main_w-overlay_w):y=(main_h-overlay_h)' watermark-video.mp4
    cd ..
}

addText()
{
    cd temp
    log "Adding text..."
    
    ffmpeg -i watermark-video.mp4 -vf \
    "format=yuv444p, \
    drawtext=fontfile=$font\\\\:style=bold:text='ODALLUS - THE DARK CALL':fontcolor=white:fontsize=60:x=150:y=h-line_h-100, \
    format=yuv420p" \
    -c:v libx264 -c:a copy -movflags +faststart "text-video.mp4"

    ffmpeg -i text-video.mp4 -vf \
    "format=yuv444p, \
    drawtext=fontfile=$font:text='JOYMASHER':fontcolor=white:fontsize=50:x=150:y=h-line_h-50, \
    format=yuv420p" \
    -c:v libx264 -c:a copy -movflags +faststart "../output/demo-reel.mp4"

    cd ..
}

log()
{
    echo "[DemoReel] $1"
}

######################
# Make the demo reel #
######################
makeDemoReel()
{
    log "Starting for video list $listFile using cut time from $cutFrom with duration of $cutDuration"
    
    clearFolders
    downloadVideos
    cutVideos
    concatVideos
    addWatermark
    addText

    log "Done."
}

listFile=$1
cutFrom=$2
cutDuration=$3

# You can change the configs below:
font="/Library/Fonts/Avenir-Heavy.ttf"
watermark="jogosdaqui.png"

makeDemoReel