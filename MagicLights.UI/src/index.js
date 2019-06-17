import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import LightClientSetup from './LightClientSetup';

let server = "https://localhost:5001/api/"

function cast() {
  fetch(server + "demo/cast_spell?colors=Red&colors=Green", {
    method: 'POST',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json'
    },
  })
}

function submit(e, clients) {
  e.preventDefault();

  let body = clients.map(client => {
    let value = client.value;
    return {
      id: client.id,
      ...value
    }
  });

  fetch(server + "configuration", {
    method: 'POST',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      LightClients: body
    })
  })
  .then(r => r.json())
  .then(() => {
    cast()
  });
}

var clients = [ ];

function onChange(value) {
  clients = value;
}

fetch(server + "configuration", {
  method: 'GET',
  headers: {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
  }
})
.then(r => r.json())
.then(r => {
  clients = r.lightClients;
  ReactDOM.render(
    <div>
      <form onSubmit={(e) => submit(e, clients)}>
        <LightClientSetup
          value={clients}
          onChange={onChange}
        />
        <br />
        <button type="submit" className="btn btn-primary">Save</button>
      </form>
    </div>,
    document.getElementById('root')
  );
})
