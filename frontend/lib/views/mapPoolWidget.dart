import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:no_cry_babies/model/map.dart';
import 'package:no_cry_babies/reducer/gameConfigReducer.dart';
import 'package:no_cry_babies/reducer/gameReducer.dart';
import 'package:no_cry_babies/state/appState.dart';

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
      StoreProvider.of<AppState>(context).dispatch(SwapTeamsAction());
    }
  }

  Color _getCardColor(CsMap map) {
    if (!map.isChecked) {
      return Colors.grey;
    }
    if (_nextMap != null && map.name == _nextMap.name) {
      return Colors.green;
    }

    return Colors.white;
  }

  Color _getBorderColor(CsMap map) {
    if (!map.isChecked) {
      return Colors.grey;
    }
    if (_nextMap != null && map.name == _nextMap.name) {
      return Colors.green;
    }

    return Colors.grey;
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
        Expanded(
          child: Container(
            child: StoreConnector<AppState, MapPool>(
              converter: (store) => store.state.gameConfigState.mapPool,
              builder: (context, mapPool) {
                return ListView.builder(
                  itemCount: mapPool.maps.length,
                  itemBuilder: (BuildContext context, int index) {
                    return Card(
                      color: _getCardColor(mapPool.maps[index]),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                        side: BorderSide(
                            color: _getBorderColor(mapPool.maps[index]),
                            width: 2),
                      ),
                      child: CheckboxListTile(
                        value: mapPool.maps[index].isChecked,
                        onChanged: (bool value) {
                          StoreProvider.of<AppState>(context).dispatch(
                              ToggleMapSelectionAction(mapPool.maps[index]));
                        },
                        secondary: Image(
                            image: AssetImage(mapPool.maps[index].imagePath)),
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
