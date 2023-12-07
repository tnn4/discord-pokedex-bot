#!/usr/bin/env bash

main() {
    echo 'hello world!'

    if [[ $# -gt 0 ]]
    then
        args=( "$@" )
        args_no_last=( ${@:1:$#-1} )
    
        for arg in "${args[@]}"
        do
            echo "$arg"
        done
    fi
    VERSION=$(<VERSION)
    # add code here
    git archive -o discord-pokedex-bot-v"${VERSION}".zip HEAD
}

main "$@"

