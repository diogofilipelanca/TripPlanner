window.renderGraph = function (graphData) {
    console.log("📊 Graph data:", graphData);

    const nodes = graphData.nodes.map(n => ({
        data: {
            id: n.id,
            label: n.id
        }
    }));

    const edges = graphData.links.map(l => ({
        data: {
            source: l.source,
            target: l.target,
            price: l.price,
            time: l.time,
            emissions: l.emissions
        }
    }));

    const cy = cytoscape({
        container: document.getElementById('graph'),
        elements: nodes.concat(edges),
        layout: { name: 'circle' },
        style: [
            {
                selector: 'node',
                style: {
                    'label': 'data(label)',
                    'background-color': '#007bff',
                    'color': '#fff',
                    'text-valign': 'center',
                    'text-halign': 'center'
                }
            },
            {
                selector: 'edge',
                style: {
                    'width': 3,
                    'line-color': '#ccc',
                    'target-arrow-color': '#ccc',
                    'target-arrow-shape': 'triangle',
                    'curve-style': 'bezier'
                }
            }
        ]
    });
};