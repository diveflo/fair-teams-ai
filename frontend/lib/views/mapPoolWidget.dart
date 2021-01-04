import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:frontend/model/map.dart';
import 'package:frontend/state/appState.dart';

class MapPoolWidget extends StatefulWidget {
  @override
  _MapPoolWidgetState createState() => _MapPoolWidgetState();
}

class _MapPoolWidgetState extends State<MapPoolWidget> {
  CsMap _nextMap;

  @override
  initState() {
    _nextMap = null;
    super.initState();
  }

  void _onNextMap() {
    List<CsMap> playableMaps = StoreProvider.of<AppState>(context)
        .state
        .gameConfigState
        .mapPool
        .getPlayableMaps();
    if (playableMaps.length > 0) {
      playableMaps.shuffle();
      setState(() {
        _nextMap = playableMaps.first;
      });
    }
  }

  Color _getBorderColor(CsMap map) {
    if (map.isDismissed) {
      return Colors.grey;
    }
    if (_nextMap != null && map.name == _nextMap.name) {
      return Colors.red;
    }

    return Colors.green;
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Container(
          margin: EdgeInsets.only(bottom: 5),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Text(
                "Maps",
                style: TextStyle(fontWeight: FontWeight.bold, fontSize: 30),
              ),
              IconButton(
                icon: Icon(Icons.cached),
                onPressed: _onNextMap,
              ),
            ],
          ),
        ),
        Container(
          child: Expanded(
            child: StoreConnector<AppState, MapPool>(
              converter: (store) => store.state.gameConfigState.mapPool,
              builder: (context, mapPool) {
                return ListView.builder(
                  itemCount: mapPool.maps.length,
                  itemBuilder: (BuildContext context, int index) {
                    return Card(
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                        side: BorderSide(
                            color: _getBorderColor(mapPool.maps[index]),
                            width: 2),
                      ),
                      child: CheckboxListTile(
                        value: mapPool.maps[index].isDismissed,
                        onChanged: (bool value) {
                          setState(() {
                            mapPool.maps[index].isDismissed =
                                !mapPool.maps[index].isDismissed;
                          });
                        },
                        secondary: Image.asset(
                          mapPool.maps[index].imagePath,
                        ),
                        title: Text(mapPool.maps[index].name),
                      ),
                    );
                  },
                );
              },
            ),
          ),
        ),
      ],
    );
  }
}
