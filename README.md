
# Savage Worlds Initiative & Deck Manager for Discord
This Discord bot can be used to manage the initiative order and card deck for a Savage World game.

# Running the Bot
1. Create a new Discord Bot
2. Build and run the executable with the "--token=<DiscordBotToken>" argument

# Initiative Instances
This bot now treats each server and channel as a separate instance. This allows you to run multiple initiative lists on one server (by using separate channels) and ensures that each server using the bot does not share one global initiative list with everyone else using the bot :)

# Managing Initiative
1. Add your actors to the initiative order with !init
2. Start the first and subsequent rounds with !round
3. End combat with !end

# Commands List
Note: If not specified, the bot will assume commands are intended for the actor the calling user last added to the initiative (not the last actor you used !set, !draw, or other commands on)
 - `!init` - Add a new actor to the initiative list
	 - By default, this adds a new actor with the user's Discord username and allegiance of "PC"
	 - Accepts custom name and allegiance parameters, eg `!init A Heroic Knight 0`
		 - Allegiance values are 0 (PC), 1 (Allied NPC), 2 (Enemy)
		 - Note: An allegiance value must be set when specifying a name longer than one word
 - `!initHidden` - Add a new actor to the initiative list, but in a hidden state
	 - Note: Hidden actors still draw cards
 - `!list` - Show the initiative list again without drawing new cards. Useful if you're chatting and the initiative order has scrolled up out of sight.
 - `!draw` - Draw a new card from the deck and discard your old one. By default, this will assume your actor name is your Discord username, or you can specify the actor with `!draw A Heroic Knight`
 - `!round` - Start a new round. All actors draw a new card. The deck is shuffled, if necessary or if a joker was hit the previous round
 - `!end` - Ends the combat, clearing the initiative order. Does not reshuffle the deck as the rulebook does not explicitly say that the deck is shuffled when starting or ending combat
 - `!hold` - Keep your card for the next round. Used when the actor doesn't take their turn and waits to act until the next round. This does not persist, if you want to continue holding your card, use this each round
 - `!remove` - Remove an actor from the initiative list. Requires an actor name to be supplied
 - `!kill` - Mark an actor as killed. Requires an actor name to be supplied. They will no longer draw cards and will always appear at the 'bottom' of the initiative
 - `!show` - Make a hidden actor visible. Requires an actor name to be supplied
 - `!set` - Used to override various values on existing actors. Accepts an actor name, attribute to change, and value to change it to
	 - Set an actor's initiative (overriding their card's initiative) with `!set <ActorName> init <Initiative Value>`. For example, `!set A Heroic Knight init 99`
	 - Set an actor's hidden state with `!set <ActorName> hidden <0 or 1>`. For example, `!set A Heroic Knight hidden 1` hides the actor
	 - Set an actor's allegiance with `!set <ActorName> allegiance <Allegiance Value>`. For example, `!set A Heroic Knight allegiance 2` to make the actor an enemy


# Notes

 - Hidden actors are still dealt cards, whether or not they're shown in the initiative order

# ToDo / Future Improvements / Bugs

 - Deck should reshuffle only if a joker is hit or if there are no cards left to deal. Currently the deck reshuffles if there aren't enough cards for all actors at the start of the round (so it is possible for a shuffle to happen before a joker is hit)
 - Make hidden actors visible to the GM via direct message. Currently hidden actors are dealt cards but no one can see what card they got because they're hidden
 - Add support for edges like Tactician that modify initiative behaviour
	 - Note: You can use the !draw card to take a new card to mimic certain edges that allow redrawing in some conditions
	 - Note: For edges/hindrances requiring two cards to be drawn, add your actor to the initiative twice and take the better/worse initiative as appropriate
 - Support for private messages for concealed information (such as who has the joker)
 - Allow users to specify a name for their character that persists so they don't need to specify it for every !init command
 - Much better error messaging and refactoring the commands so they're more intuitive (allowing "true/false" or "PC/NPC/Enemy" as valid parameters instead of only numeric values)
