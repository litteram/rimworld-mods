#! /usr/bin/env nix-shell
#! nix-shell --impure -i fish -p steamcmd

set app_id 294100

for project in */
    set project_name (basename "$project")
    set published_file_id $project_name/About/PublishedFileId.txt
    if not -f $published_file_id
        continue
    end

    set published_file_id_data (cat $published_file_id)
    echo "workshopitem
    {
        appid "$app_id"
        publishedfileid "$published_file_id_data"
        contentfolder "$(pwd)/$project_name"
    }
    " >"$project_name.vdf"

    steamcmd \
        +login $STEAM_USERNAME $STEAM_PASSWORD \
        +workshop_build_item $(pwd -P)/$project_name.vdf \
        +quit
end
