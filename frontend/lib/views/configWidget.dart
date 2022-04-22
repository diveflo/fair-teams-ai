import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:no_cry_babies/reducer/gameConfigReducer.dart';
import 'package:no_cry_babies/state/appState.dart';

class ConfigWidget extends StatelessWidget {
  const ConfigWidget({Key key}) : super(key: key);

  _onValueChanged(BuildContext context) {
    StoreProvider.of<AppState>(context).dispatch(ToggleIsConfigVisibleAction());
  }

  @override
  Widget build(BuildContext context) {
    return StoreConnector<AppState, bool>(
        converter: (store) => store.state.gameConfigState.isConfigVisible,
        builder: (context, isConfigVisible) {
          return Padding(
            padding: const EdgeInsets.symmetric(vertical: 15),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text(
                  "Config",
                  style: TextStyle(fontWeight: FontWeight.bold),
                ),
                Switch(
                  activeColor: Theme.of(context).highlightColor,
                  activeTrackColor: Theme.of(context).primaryColor,
                  inactiveTrackColor: Theme.of(context).primaryColor,
                  value: isConfigVisible,
                  onChanged: (bool isOn) {
                    _onValueChanged(context);
                  },
                ),
              ],
            ),
          );
        });
  }
}
