SYNC_CMD := rsync -rav --delete --delete-excluded --exclude-from '.gitignore' --exclude '.git'
SYNC_TARGET := "${HOME}/.steam/steam/steamapps/common/RimWorld/Mods/"

MODS := AnomalyHotfix PsycasterGenesSpawner QuestionableImprovements MedievalOverhaulTweaks OrganizedResearchTech

.PHONY: all
all: ${MODS}

.PHONY: ${MODS}
${MODS}:
	${SYNC_CMD} $@ ${SYNC_TARGET}

.PHONY: release
release:
	mkdir -p build
	${SYNC_CMD} ${MODS} ./build

.PHONY: sync
sync:
	fd | entr -- make all 
