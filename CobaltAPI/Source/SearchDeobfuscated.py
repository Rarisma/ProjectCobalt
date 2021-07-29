from igdb.wrapper import IGDBWrapper
import sys, json
wrapper = IGDBWrapper("", "")
#The sky is a neighborhood
search = wrapper.api_request(
            'games',
            'search "' + sys.argv[1] + '"; fields age_ratings.rating,cover.*,category,cover,dlcs,franchise,genres,player_perspectives,platforms,storyline,name;'
            )
test = json.loads(search)

try:
    print("Name: " + str(test[0]["name"]))
except:
    print("Name: Unknown")

try:
    print("Age Rating: " + str(test[0]["age_ratings"][0]['rating']))
except:
    print("Age Rating: Unknown")

try:
    print("Cover URL: " + test[0]["cover"]['url'])
except:
    print("Cover URL: Unknown")

try:
    print("Story: " + str(test[0]["storyline"]))
except:
    print("Story: Unknown")

print("Story: " + str(test[0]["storyline"]))


