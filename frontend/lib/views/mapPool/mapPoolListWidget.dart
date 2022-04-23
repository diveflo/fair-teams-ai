import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:no_cry_babies/model/map.dart';
import 'package:no_cry_babies/reducer/gameConfigReducer.dart';
import 'package:no_cry_babies/state/appState.dart';
import 'package:no_cry_babies/views/mapPool/mapWidget.dart';

class MapPoolListWidget extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    Color _getCardColor(CsMap map) {
      var _nextMap =
          StoreProvider.of<AppState>(context).state.gameConfigState.nextMap;
      if (!map.isChecked) {
        return Colors.grey;
      }
      if (_nextMap != null && map.name == _nextMap.name) {
        return Colors.green;
      }

      return Colors.white;
    }

    Color _getBorderColor(CsMap map) {
      var _nextMap =
          StoreProvider.of<AppState>(context).state.gameConfigState.nextMap;
      if (!map.isChecked) {
        return Theme.of(context).primaryColor;
      }
      if (_nextMap != null && map.name == _nextMap.name) {
        return Colors.green;
      }

      return Theme.of(context).primaryColor;
    }

    return Container(
      child: StoreConnector<AppState, MapPool>(
        converter: (store) => store.state.gameConfigState.mapPool,
        builder: (context, mapPool) {
          return ListView.builder(
            controller: ScrollController(),
            itemCount: mapPool.maps.length,
            itemBuilder: (BuildContext context, int index) {
              return Card(
                color: _getCardColor(mapPool.maps[index]),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8),
                  side: BorderSide(
                      color: _getBorderColor(mapPool.maps[index]), width: 2),
                ),
                child: CheckboxListTile(
                  checkColor: Theme.of(context).primaryColor,
                  activeColor: Theme.of(context).highlightColor,
                  value: mapPool.maps[index].isChecked,
                  onChanged: (bool value) {
                    StoreProvider.of<AppState>(context).dispatch(
                        ToggleMapSelectionAction(mapPool.maps[index]));
                  },
                  secondary: GestureDetector(
                      onTap: () async {
                        if (mapPool.maps[index].imageMapCallsPath != null) {
                          await showDialog(
                              context: context,
                              builder: (_) => MapWidget(
                                  mapPool.maps[index].imageMapCallsPath));
                        }
                      },
                      child: Image(
                          image: AssetImage(mapPool.maps[index].imagePath))),
                  title: Text(mapPool.maps[index].name),
                ),
              );
            },
          );
        },
      ),
    );
  }
}
