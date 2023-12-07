#!/bin/bash


me=$(basename $0)

force=false

## ARGUMENTS ##
# Process arguments/flags/switches here
process_args () {
    # if there are no arguments, do nothing
    if [[ $# -eq 0 ]];then
        return
    fi

    args_no_last=${@:1:$#-1}
    arg_last=${@: -1}

    array_len=${#args_no_last[@]}
    idx=0
    for i in $array_len;do
        echo "idx: $i"
    done

    for arg in $args_no_last; do
        # Process specific args here
        if [[ $arg =~ ^ABC$ ]]; then
            echo "ABC"
        fi
        case $arg in
            # https://stackoverflow.com/questions/15028567/get-the-index-of-a-value-in-a-bash-array
            "-o")
                output_zip=""
            ;;
            "-F")
                force=true
                echo "Packaging..."
                target_file=${arg_last}
                output_zip=web_"${target_file}".zip
                zip -r "${output_zip}" art "${target_file}" mk-phaser-project.sh package.sh phaser-arcade-physics.min.js play.sh todo.md
                echo "DONE"
                exit
            ;;
            *)
                echo "unknown arg"
                help_more
            ;;
        esac
        idx=$(( idx+1 ))
    done
} 
# end process_args()


## HELP ##
# put help and usage instructions here
help_more() {
    # write usage instructions here
    echo "e.g. usage: $me --options"
}

help() {
    # write usage instructions here
    echo "e.g. usage: $me --options"
    echo "usage: $me <folder-name>"
}

if [[ $1 == "-h" || $1 == "--help" ]];then
    help_more
    exit
fi

# END HELP

function join_by {
  local d=${1-} f=${2-}
  if shift 2; then
    printf %s "$f" "${@/#/$d}"
  fi
}

## MAIN ##



# put main code here
main() {
    
    # in_file=${1}
    output_zip=$1

    target_files=( 
        attic
        img
        secure
        secureExample
        src
        .git
        design.md
        Directory.Build.Props
        dotnetbot1.csproj
        License
        package.sh
        README.md
        wiki.md
        .gitignore
    )
    flattened_array=$(join_by ' ' "${target_files[@]}")
    if [[ $force = true ]];then
        zip -r "${output_zip}" "${flattened_array}"
    fi

    if [[ -f "${output_zip}" ]]; then
        echo "file already exists, do you want to overwrite (y/n)?"
        read -r input
        if [[ $input == "yes" || $input == "y" ]];then
            :
        else
            exit
        fi
    fi

    echo "[$me]: packaging..."
    process_args "$@"
    zip -r "${output_zip}" . "${flattened_array}"
}

# import functions only and don't execute if --source-only is present
if [ "${1}" = "--source-only" ];then
    echo 'Functions imported. Script not executed'
else
    if [[ $# -eq 0 ]];then
        help
        exit
    fi
    main "$@"
fi

# END MAIN
