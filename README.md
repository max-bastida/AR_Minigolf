# COSC477 Assignment - Minigolf

## About

// TODO: This should be updated as development continues. For now this is just the game idea

### General idea:

- Use tiles to build a golf course
- App generates 3D course from tiles
- Use app to aim and hit golf balls along the course
- Players can take turns by passing the phone around. App will record the location of each player’s
ball and their score
- Rearranging the tiles will give a different hole – may be some rules about what tiles should be
used to make more or less challenging holes
  - Procedural generation could be utilized to add virtual obstacles as well, with the user
regenerating the course as needed before locking it in and playing

### Physical pieces:

- Cards will be used to build the golf course
	- Cards will be placed apart from each other and the app will generate the course inbetween
- They will be sized so that the golf course can be built on a tabletop
- The height of a card’s virtual counterpart will be determined by the height the physical card is
placed relative to the start card
	- Players can make hills by putting directional pieces on real world objects
	- Obstacle size can be altered based on the height its card is placed at

### Gameplay:

- Aim by point/swipe (e.g., common in pool games) OR angle of phone relative to course
- Press and hold to determine power of shot
- Golf ball controlled by physics

![Game Idea Image](https://eng-git.canterbury.ac.nz/aco155/cosc477-assignment/-/wikis/uploads/6b99a7c0864b9a2d8839ead381667cfc/Game_Idea_Image.jpg "An image showing some examples of how the game could look, including the tiles, and a physical and phone view.")

## Setup

### Player Setup

// TODO: Installation guide, running guide

### Dev Setup

Unity 2022.3.8f1 is used for development
// TODO: Anything more that we use for development can go here to ensure versions are synced

## Trello

[Trello Link](https://trello.com/b/7ccykE1n)

Kanban strategy:
- Top left card in each column always has most importance
- Cards should only ever move right
- Limits for WIP (// TODO: can change, also should be automated at some point):
	- Doing: 3
	- Reviewing: 2
	- Testing: 1
- Reviewing & testing should be done by at least 1 other person than the main developer of a feature
- Cards have a "Pick Up" and "Put Down" button (if you go inside the card). These are used for assigning cards to do to yourself, or putting down cards for others to pick up (denoted by the green label)
- // TODO: Maybe something on archiving to a different board to avoid clutter? Same with triaging? Work out if needed

## Git Strategy

- Feature based branching from `dev` (//TODO: Prefixed with bug/, feature/, etc.?)
- Create a merge request (following the MR template) when done, move the Trello card to the reviewing column
- Each 'release' we tag with a version number, and merge `dev` into `master`

## Contributors

Andrew Cook

Gabby Rosemergy

Max Bastida

