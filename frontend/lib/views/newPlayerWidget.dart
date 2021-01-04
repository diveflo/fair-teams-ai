import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:frontend/model/candidate.dart';
import 'package:frontend/reducer/gameConfigReducer.dart';
import 'package:frontend/state/appState.dart';
import 'package:frontend/views/playerSelection.dart';
import 'package:frontend/views/scrambleWidget.dart';

class NewPlayerColumnWidget extends StatefulWidget {
  @override
  _NewPlayerColumnWidgetState createState() => _NewPlayerColumnWidgetState();
}

class _NewPlayerColumnWidgetState extends State<NewPlayerColumnWidget> {
  bool _isValid;

  TextEditingController _nameController;
  TextEditingController _steamIdController;

  @override
  void initState() {
    _nameController = TextEditingController();
    _steamIdController = TextEditingController();
    _steamIdController.addListener(_validateSteamID);
    _isValid = false;

    super.initState();
  }

  void _validateSteamID() {
    setState(() {
      if (_steamIdController.value.text.length == 17) {
        _isValid = true;
      } else {
        _isValid = false;
      }
    });
  }

  void _addPlayer() {
    if (_isValid) {
      StoreProvider.of<AppState>(context).dispatch(AddPlayerAction(Candidate(
          name: _nameController.text, steamID: _steamIdController.text)));
      setState(() {
        _nameController.clear();
        _steamIdController.clear();
      });
    }
  }

  @override
  void dispose() {
    _nameController.dispose();
    _steamIdController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.start,
        children: [
          Padding(
            padding: EdgeInsets.only(bottom: 10),
            child: Text(
              "New Player",
              style: TextStyle(fontWeight: FontWeight.bold, fontSize: 20),
            ),
          ),
          Padding(
            padding: EdgeInsets.only(bottom: 10),
            child: TextField(
              controller: _nameController,
              decoration: InputDecoration(
                  border: OutlineInputBorder(), labelText: "Name"),
            ),
          ),
          TextField(
            controller: _steamIdController,
            keyboardType: TextInputType.number,
            inputFormatters: [FilteringTextInputFormatter.digitsOnly],
            decoration: InputDecoration(
                border: OutlineInputBorder(),
                labelText: "SteamID",
                errorText: _isValid ? null : "invalid steam ID"),
          ),
          Expanded(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                MyButton(
                  onPressed: _addPlayer,
                  color: Colors.pink,
                  buttonText: "Add Player",
                ),
                ScrambleWidget(),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
