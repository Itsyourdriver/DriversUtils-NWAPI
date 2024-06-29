# DriversUtils

A SCPSL NorthwoodAPI (NWAPI) plugin purpose built for a private server. 
Disclaimer: Builds will not be provided, you can compile yourself OR wait for me to release a less jank version in the future.

# Dependinces 
- Requires https://github.com/brayden-dowson/TheRiptide/ 's Utility.dll & Cedmod's SCPSLAudioAPI. 
- I'm not using them, but you may expect errors due to slocloader-nw.dll as I left it in my references. If you need it please go look for them in the NWApi Discord Server or on github.

# Features

### Custom Items
- SCP 1499.
- Resurrection Pills (actually hilarous, need to turn up the chance for this lol)
- Random Class Pills (still figuring out how this'll work, might just turn it into a betrayal pill)
- + More soon.

### SCP-294
- Has lots of drinks from SCP: Containment Breach. Not balanced at all but that's what makes it fun.
- The machine model is not in this plugin, we are using slocLoader to create the physical machine in both EZ and LCZ.

### Serpents Hand
- Has two classes, The captain and the agent. Captains are beefier and agents are the standard class.

### PcBuff (mostly disabled as of 6/29/24)
- Adds 2 commands to assist SCP-079 very slightly, .blackout isn't very useful (as you need a higher tier, it has a high cooldown, and uses a lot of power).
- finally .findally or .findallies (or the other aliases i forgot about) allows you to see the current room of serpents hand for better cooperation because they usually don't have a way to communicate unless they find eachother.

### Subclasses
- MTF Nu-7 can spawn. They have 10 more hp, guaranteed better gear, and are basically just epsilon-11 on steroids. :D (now disableable in config, not in this repo yet)
- The Science Team can spawn. (don't ask)
- Facility Guard Captain class which is a beefed up guard class that is put in place to try and balance out guards for lower players. Notable things are that they start with an mtf prviate keycard.
- Adds "The Kid", whom spawns with candy and is shorter than everyone else. This was fun during the christmas event so we could have voice effects using the cake :)
- Adds the MTF Boom Boom Boy (would've been called Demolitionist but friends voted otherwise). Self explanatory.
- Adds the Chaos Specialist Subclass. They get one special SCP item.
- Adds the Senior Researcher. They get a better card.

### Other Changes
- Coins get "randomized" (changed into a random item from a list) on very fine.
- Chaos will be forced if mtf spawn twice in a row. Mostly fixes some of the issues for **low** player servers. (5-7 plr servers, once there are 2 scps it changes tho)
- Custom Cassie Messages for multiple different ingame events.
- CustomInfo/Role names when you hover over someone who is a custom class. (DISABLED DUE TO UNFORSEEN ISSUES)
- SCP-049-2 instances have their height randomized

Have fun!
