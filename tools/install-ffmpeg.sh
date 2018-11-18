brew update
brew upgrade --all
brew cleanup

brew reinstall ffmpeg --force --with-freetype --with-fontconfig
brew link ffmpeg