import 'package:flutter/material.dart';
import 'package:no_cry_babies/views/mapPool/mapPoolWidget.dart';
import 'package:no_cry_babies/views/scrambleTeamsView.dart';

/// This class creates the app layout for small screen sizes
class SmallAppLayoutWidget extends StatefulWidget {
  final title;

  const SmallAppLayoutWidget(
    this.title, {
    Key key,
  }) : super(key: key);

  @override
  State<SmallAppLayoutWidget> createState() => _SmallAppLayoutWidgetState();
}

class _SmallAppLayoutWidgetState extends State<SmallAppLayoutWidget> {
  int _selectedIndex = 0;

  static const List<Widget> _widgetOptions = <Widget>[
    ScrambleTeamsView(),
    MapPoolWidget(),
  ];

  void _onItemTapped(int index) {
    setState(() {
      _selectedIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        // Here we take the value from the MyHomePage object that was created by
        // the App.build method, and use it to set our appbar title.
        title: Text(widget.title),
        leading: Image(image: AssetImage("assets/hnyb.jpg")),
      ),
      body: Center(
        // Center is a layout widget. It takes a single child and positions it
        // in the middle of the parent.
        child: _widgetOptions.elementAt(_selectedIndex),
      ),
      bottomNavigationBar: BottomNavigationBar(
        backgroundColor: Colors.black,
        items: const <BottomNavigationBarItem>[
          BottomNavigationBarItem(
            icon: Icon(Icons.shuffle_rounded),
            label: 'Scramble',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.info_outline_rounded),
            label: 'Maps',
          ),
        ],
        currentIndex: _selectedIndex,
        selectedItemColor: Colors.orange,
        unselectedItemColor: Colors.white,
        onTap: _onItemTapped,
      ),
    );
  }
}
